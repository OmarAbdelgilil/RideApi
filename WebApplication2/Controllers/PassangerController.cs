using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApplication1.Data;
using WebApplication2.Auth;
using WebApplication2.Dtos;
using WebApplication2.Migrations;
using WebApplication2.Models;
using WebApplication2.RealTime;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassangerController : Controller
    {

        private readonly IDataRepository<Passanger> _PassangerRepository;
        private readonly IDataRepository<Driver> _DriverRepository;
        private readonly IDataRepository<Rides> _RidesRepository;
        private readonly IHubContext<SignalRHub> _HubContext;
        private readonly IDataRepository<Credentials> _CredentialsRepository;
        private readonly AuthInterface _auth;
        private readonly IMapper _mapper;

        public PassangerController(
             IDataRepository<Passanger> PassangerRepository,
        IDataRepository<Credentials> CredentialsRepository,
        IDataRepository<Driver> DriverRepository,
        IDataRepository<Rides> RidesRepository,
        IHubContext<SignalRHub> HubContext,
        IDataRepository<Credentials> Credentials,
        AuthInterface auth,
        IMapper mapper
            )
        {
            _PassangerRepository = PassangerRepository;
            _auth = auth;
            _DriverRepository = DriverRepository;
            _RidesRepository = RidesRepository;
            _HubContext = HubContext;
            _CredentialsRepository = CredentialsRepository;
            _mapper = mapper;


        }

        private double CalculateDistance(double lon1, double lat1, double lon2, double lat2)
        {
            const double R = 6371.0; // Radius of the Earth in kilometers

            // Convert latitude and longitude from degrees to radians
            double lon1Rad = ToRadians(lon1);
            double lat1Rad = ToRadians(lat1);
            double lon2Rad = ToRadians(lon2);
            double lat2Rad = ToRadians(lat2);

            // Haversine formula
            double dLon = lon2Rad - lon1Rad;
            double dLat = lat2Rad - lat1Rad;
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Calculate the distance
            double distance = R * c;
            return distance;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }


        //[Authorize(Roles = "Passenger,Admin")]
        [HttpGet("getPassengerByEmail/{email}")]
        public async Task<IActionResult> GetPassengerByEmail(string email)
        {
            Passanger passanger = await _PassangerRepository.GetByEmailAsync(email);
            await _DriverRepository.GetAllAsync();
            if (passanger == null) { return NotFound("no passanger with this email found"); }
            await _RidesRepository.GetAllAsync();
            if(passanger.Rides == null) return Ok(passanger);
            foreach (var ride in passanger.Rides!)
            {
                ride.Driver!.Rides = null;
            }
            return Ok(passanger);

        }
        //[Authorize(Roles = "Admin")]
        [HttpDelete("deletePassenger/{email}")]
        public async Task<IActionResult> DeletePassenger(String email)
        {
            Passanger checkPassenger = await _PassangerRepository.GetByEmailAsync(email);
            Credentials checkCredentials = await _CredentialsRepository.GetByEmailAsync(email);
            if (checkPassenger == null || checkCredentials == null) { return NotFound("no passanger with this eamil found"); }
            await _CredentialsRepository.DeleteAsyncByEmail(email);
            await _PassangerRepository.DeleteAsyncByEmail(email);
            await _PassangerRepository.Save();
            await _CredentialsRepository.Save();
            return Ok();
        }
        [HttpPost("createPassenger")]
        public async Task<IActionResult> CreatePassenger(NewPassengerDto newPassanger)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (newPassanger.Password != newPassanger.ConfirmPassword)
            {
                return BadRequest("Password doesn't match");
            }
            if(!await (_auth.Register(newPassanger.Email!,newPassanger.Password!,newPassanger.Role)))
                {
                return BadRequest("Email already exists");
            }
            Passanger passanger = _mapper.Map<Passanger>(newPassanger);
            Dictionary<string, dynamic> toSend = new Dictionary<string, dynamic>();
            toSend.Add("role", "Passenger");
            toSend.Add("data", passanger);
            await _PassangerRepository.AddAsync(passanger);
            await _PassangerRepository.Save();
            await _HubContext.Clients.All.SendAsync("UpdatePending", toSend);

            return Ok(passanger);
        }
   
        //[Authorize(Roles = "Passenger,Admin")]
        [HttpPatch("updatePassenger")]
        public async Task<IActionResult> UpdatePassenger(String email, String fieldToUpdate, String newValue)
        {
            Passanger? passenger = await _PassangerRepository.GetByEmailAsync(email);
            if (passenger == null)
            {
                return NotFound("No passangers found");
            }
            if (fieldToUpdate == "email")
            {
                return BadRequest("can't update the email");
            }
            switch (fieldToUpdate.ToLower())
            {
                case "username":
                    passenger.UserName = (string)newValue;
                    break;
                case "gender":
                    passenger.Gender = Int32.Parse(newValue);
                    break;
                // Add cases for other fields as needed
                default:
                    return BadRequest("Invalid field to update");

            }

            await _PassangerRepository.UpdateAsync(passenger);
            await _PassangerRepository.Save();
            return Ok(passenger);
        }
        [HttpPost("tripPrice")]
        public  IActionResult TripPrice(PriceDto directions)
        {
            return Ok(CalculateDistance(directions.Long1, directions.Lat1, directions.Long2, directions.Lat2) * 10);
        }
        [HttpPost("requestRide")]
        public async Task<IActionResult> RequestRide(RequestRideDto ride)
        {

            double price = CalculateDistance(ride.Long1, ride.Lat1, ride.Long2, ride.Lat2) * 10;
            Driver driver = await _DriverRepository.GetByEmailAsync(ride.DriverEmail!);
            if (await _PassangerRepository.GetByEmailAsync(ride.PassangerEmail!) == null || driver == null)
            {
                return NotFound("driver or passenger account not found");
            }
            if (driver.Availability == false)
            {
                return BadRequest("driver not found");
            }
            DateTime today = DateTime.Now;
            string dateString = today.ToString("yyyy-MM-dd");
            Rides rideCreated = _mapper.Map<Rides>(ride);
            rideCreated.Date = dateString;
            rideCreated.Price = Math.Round(price,2);
            rideCreated.Status = "pending";
            await _RidesRepository.AddAsync(rideCreated);
            await _RidesRepository.Save();
            Dictionary<String, dynamic> data = new Dictionary<String, dynamic>();
            data.Add("type", "rideCreated");
            
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            data.Add("data", JsonSerializer.Serialize(rideCreated, options));
            await _HubContext.Clients.All.SendAsync(ride.DriverEmail!, JsonSerializer.Serialize(data));
            await _HubContext.Clients.All.SendAsync("ridesUpdated", data);
            return Ok(rideCreated);

        }

        [HttpPatch("cancelRide")]
        public async Task<IActionResult> CancelRide(String id)
        {
            Rides ride = await _RidesRepository.GetByEmailAsync(id);
            if (ride == null) { return NotFound(); }
            if (ride.Status == "done" || ride.Status == "paid" || ride.Status == "cancelled")
            {
                return BadRequest("Ride is already done");
            }
            if(ride.Status == "ongoing")
            {
                Driver driver = await _DriverRepository.GetByEmailAsync(ride.DriverEmail!);
                driver.Availability = true;
                await _DriverRepository.UpdateAsync(driver);
                await _DriverRepository.Save();
            }
            ride.Status = "cancelled";
            await _RidesRepository.UpdateAsync(ride);
            await _RidesRepository.Save();
            Dictionary<String, dynamic> data = new Dictionary<String, dynamic>();
            data.Add("type", "rideUpdated");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            data.Add("data", JsonSerializer.Serialize(ride, options));
            await _HubContext.Clients.All.SendAsync(ride.DriverEmail!, JsonSerializer.Serialize(data));
            await _HubContext.Clients.All.SendAsync("ridesUpdated", data);
            return Ok(ride);

        }
        //[Authorize(Roles = "Passenger")]
        [HttpPatch("payAndFeedback")]
        public async Task<IActionResult> PayAndFeedback(String id, int rating , string? feedback) {
            Rides ride = await _RidesRepository.GetByEmailAsync(id);
            if (ride == null) { return NotFound(); }
            if (ride.Status != "done")
            {
                return BadRequest("Something went wrong");
            }
            ride.Status = "paid";
            if(rating!=0)
            {
                Driver driver = await _DriverRepository.GetByEmailAsync(ride.DriverEmail!);
                if (driver == null) { return NotFound();}
                if (driver.Rating != 0)
                {
                    driver.Rating = (driver.Rating + rating) / 2;
                }
                else
                {
                    driver.Rating = rating;
                }
                ride.Rate = rating;
                await _DriverRepository.UpdateAsync(driver);
                await _DriverRepository.Save();
            }
            if(feedback!=null)
            {
                ride.Feedback = feedback;
            }
            await _RidesRepository.UpdateAsync(ride);
            await _DriverRepository.Save();
            Dictionary<String, dynamic> data = new Dictionary<String, dynamic>();
            data.Add("type", "rideUpdated");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            data.Add("data", JsonSerializer.Serialize(ride, options));
            await _HubContext.Clients.All.SendAsync(ride.DriverEmail!, JsonSerializer.Serialize(data));
            await _HubContext.Clients.All.SendAsync("ridesUpdated", data);
            return Ok(ride);
        }

        [HttpGet("/getCities")]
        public IActionResult GetCities()
        {
            return Ok(
                MapData.GetMap()
                );
        }


    }
}
