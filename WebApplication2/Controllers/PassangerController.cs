using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using WebApplication1.Data;
using WebApplication2.Dtos;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassangerController : Controller
    {

        private readonly IDataRepository<Passanger> _PassangerRepository;
        private readonly IDataRepository<Credentials> _CredentialsRepository;
        private readonly IDataRepository<Driver> _DriverRepository;
        private readonly IDataRepository<Rides> _RidesRepository;

        public PassangerController(
             IDataRepository<Passanger> PassangerRepository,
        IDataRepository<Credentials> CredentialsRepository,
        IDataRepository<Driver> DriverRepository,
        IDataRepository<Rides> RidesRepository
            )
        {
            _PassangerRepository = PassangerRepository;
            _CredentialsRepository = CredentialsRepository;
            _DriverRepository = DriverRepository;
            _RidesRepository = RidesRepository;

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

        private  double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        [HttpGet("getAllPassengers")]
        public async Task<IActionResult> GetAllPassengers()
        {
            var passangers = (await _PassangerRepository.GetAllAsync()).ToList();
            if (passangers == null || !passangers.Any())
            {
                return NotFound("No passangers found");
            }
            foreach (var passanger in passangers)
            {
                passanger.Rides = (await _RidesRepository.GetAllAsync()).Where(r=>r.PassangerEmail == passanger.Email).ToList();
            }
            return Ok(passangers);
        }

        [HttpGet("getPassengerByEmail/{email}")]
        public async Task<IActionResult> GetPassengerByEmail(string email)
        {
            Passanger passanger = await _PassangerRepository.GetByEmailAsync(email);
            if (passanger == null) { return NotFound("no passanger with this email found"); }
            passanger.Rides = (await _RidesRepository.GetAllAsync()).Where(r=>r.PassangerEmail == email).ToList();
            return Ok(passanger);

        }
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


            Credentials credentials = new Credentials()
            {
                Email = newPassanger.Email,
                Password = newPassanger.Password,
                Role = newPassanger.Role
            };
            if((await _CredentialsRepository.GetByEmailAsync(credentials.Email!)) != null)
            {
                return BadRequest("Email already exists");
            }
            Passanger passanger = new Passanger()
            {
                Email = newPassanger.Email,
                UserName = newPassanger.UserName,
                Gender = newPassanger.Gender,
            };

            await _CredentialsRepository.AddAsync(credentials);
            await _CredentialsRepository.Save();
            await _PassangerRepository.AddAsync(passanger);
            await _PassangerRepository.Save();


            return Ok(passanger);
        }
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(String email, String old, String newPassword)
        {
            Credentials? passenger = await _CredentialsRepository.GetByEmailAsync(email);
            if (passenger == null)
            {
                return NotFound("No passangers found");
            }
            if (passenger.Password == old)
            {
                passenger.Password = newPassword;
                await _CredentialsRepository.UpdateAsync(passenger);
                await _CredentialsRepository.Save();
                return Ok();
            }
            else
            {
                return BadRequest("old password is wrong");
            }
        }
        [HttpPost("updatePassenger")]
        public async Task<IActionResult> UpdatePassenger(String email, String fieldToUpdate, String newValue)
        {
            Passanger? passenger = await _PassangerRepository.GetByEmailAsync(email);
            if (passenger == null)
            {
                return NotFound("No passangers found");
            } if (fieldToUpdate == "email")
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
        [HttpPost("requestRide")]
        public async Task<IActionResult> requestRide(RequestRideDto ride) {

            double price = CalculateDistance(ride.Long1, ride.Lat1, ride.Long2, ride.Lat2) * 10;
            Driver driver =  await _DriverRepository.GetByEmailAsync(ride.DriverEmail!);
            if (await _PassangerRepository.GetByEmailAsync(ride.PassengerEmail!) == null || driver == null)
            {
                return NotFound("driver or passenger account not found");
            }
            if(driver.Availability == false)
            {
                return BadRequest("driver not found");
            }
            DateTime today = DateTime.Now;
            string dateString = today.ToString("yyyy-MM-dd");
            Rides rideCreated = new Rides()
            {
                Date = dateString,
                DriverEmail = ride.DriverEmail,
                PassangerEmail = ride.PassengerEmail,
                Price = price,
                Status = "pending",
                From = ride.From,
                To = ride.To,
            };
            await _RidesRepository.AddAsync(rideCreated);
            await _RidesRepository.Save();
            return Ok(rideCreated);
           
        }
        
    }
}
