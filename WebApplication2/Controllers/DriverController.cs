using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Helpers;
using WebApplication2.Auth;
using WebApplication2.Dtos;
using WebApplication2.Models;
using WebApplication2.RealTime;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : Controller
    {
        private readonly IDataRepository<Driver> _DriverRepository;
        private readonly IDataRepository<Rides> _RidesRepository;
        private readonly IDataRepository<Passanger> _PassangerRepository;
        private readonly IDataRepository<Credentials> _CredentialsRepository;
        private readonly IHubContext<SignalRHub> _HubContext;
        private readonly AuthInterface _auth;
        private readonly IMapper _mapper;
        public DriverController(
             IDataRepository<Driver> DriverRepository,
        IDataRepository<Rides> RidesRepository,
        IDataRepository<Passanger> PassangerRepository,
        IDataRepository<Credentials> CredentialsRepository,
        IHubContext<SignalRHub> HubContext,
        AuthInterface auth,
        IMapper mapper
            )
        {
            _DriverRepository = DriverRepository;
            _auth = auth;
            _PassangerRepository =  PassangerRepository;
            _RidesRepository = RidesRepository;
            _HubContext = HubContext;
            _CredentialsRepository = CredentialsRepository;
            _mapper = mapper;
        }


        [Authorize(Roles = "Driver,Admin")]
        [HttpGet("getDriverByEmail/{email}")]
        public async Task<IActionResult> GetDriverByEmail(string email)
        {
            await _PassangerRepository.GetAllAsync();
            Driver driver= await _DriverRepository.GetByEmailAsync(email);
            if (driver == null) { return NotFound("no passanger with this email found"); }
            await _RidesRepository.GetAllAsync();
            if(driver.Rides == null)   return Ok(driver);
            foreach (var ride in driver.Rides!)
            {
                ride.Passanger!.Rides = null;
            }
            return Ok(driver);

        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("deleteDriver/{email}")]
        public async Task<IActionResult> DeleteDriver(String email)
        {
            Driver checkDriver = await _DriverRepository.GetByEmailAsync(email);
            Credentials checkCredentials = await _CredentialsRepository.GetByEmailAsync(email);
            if (checkDriver == null || checkCredentials == null) { return NotFound("no passanger with this eamil found"); }
            await _CredentialsRepository.DeleteAsyncByEmail(email);
            await _DriverRepository.DeleteAsyncByEmail(email);
            await _DriverRepository.Save();
            await _CredentialsRepository.Save();
            return Ok();
        }

        [HttpPost("createDriver")]
        public async Task<IActionResult> CreateDriver([FromForm] NewDriverDto newDriver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (newDriver.Password != newDriver.ConfirmPassword)
            {
                return BadRequest("Password doesn't match");
            }


            if (!await (_auth.Register(newDriver.Email!, newDriver.Password!, newDriver.Role)))
            {
                return BadRequest("Email already exists");
            }

            String fileName = "";
            if (newDriver.File != null)
            {
                var result = UploadHandler.Upload(newDriver.File, "drivers");
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    return BadRequest(new
                    {
                        message = result.ErrorMessage
                    });
                }
                fileName = result.FileName;
            }
            Driver driver = _mapper.Map<Driver>(newDriver);
            driver.ImagePath = fileName;
            await _DriverRepository.AddAsync(driver);
            await _DriverRepository.Save();
            Dictionary<string, dynamic> toSend = new Dictionary<string, dynamic>();
            toSend.Add("role", "Driver");
            toSend.Add("data", driver);
            await _HubContext.Clients.All.SendAsync("UpdatePending", toSend);
            return Ok(driver);
        }
      /*  //[Authorize(Roles = "Driver,Admin")]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto data)
        {
            Credentials? driver = await _CredentialsRepository.GetByEmailAsync(data.Email!);
            if (driver == null)
            {
                return NotFound("No Drivers found");
            }
            if (driver.Password == data.OldPassword)
            {
                driver.Password = data.NewPassword;
                await _CredentialsRepository.UpdateAsync(driver);
                await _CredentialsRepository.Save();
                return Ok();
            }
            else
            {
                return BadRequest("old password is wrong");
            }
        }
      */
        [Authorize(Roles = "Driver,Admin")]
        [HttpPatch("updateDriver")]
        public async Task<IActionResult> UpdateDriver(String email, String fieldToUpdate, String newValue)
        {
            Driver? driver = await _DriverRepository.GetByEmailAsync(email);
            if (driver == null)
            {
                return NotFound("No drivers found");
            }
            if (fieldToUpdate == "email")
            {
                return BadRequest("can't update the email");
            }
            switch (fieldToUpdate.ToLower())
            {
                case "username":
                    driver.Username = (string)newValue;
                    break;
                case "gender":
                    driver.Gender = Int32.Parse(newValue);
                    break;
                case "carType":
                    driver.CarType = (string)newValue;
                    break;
                case "city":
                    driver.City = (string)newValue;
                    break;
                case "smoking":
                    
                    driver.Smoking = newValue.ToLower() == "true" ? true : false; ;
                    break;
                case "region":
                    driver.Region = (string)newValue;
                    break;
                case "availability":
                    driver.Availability = newValue.ToLower() == "true" ? true : false;
                    break;
                case "blocked":
                    driver.Smoking = newValue.ToLower() == "true"? true : false;
                    break;
                case "rating":
                    driver.Rating = Convert.ToDouble(newValue);
                    break;
                // Add cases for other fields as needed
                default:
                    return BadRequest("Invalid field to update");
            }

            await _DriverRepository.UpdateAsync(driver);
            await _DriverRepository.Save();
            await _HubContext.Clients.All.SendAsync("driversUpdated", "");
            return Ok(driver);
        }
        [Authorize(Roles = "Driver,Admin")]
        [HttpPatch("rejectRide")]
        public async Task<IActionResult> RejectRide(String id)
        {
            Rides ride = await _RidesRepository.GetByEmailAsync(id);
            if (ride == null) { return NotFound(); }
            if (ride.Status == "done" || ride.Status == "paid" || ride.Status == "cancelled")
            {
                return BadRequest("Ride is already done");
            }
            if (ride.Status == "ongoing")
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

            await _HubContext.Clients.All.SendAsync(ride.PassangerEmail!, ride.Status);
            await _HubContext.Clients.All.SendAsync("ridesUpdated", data);
            return Ok(ride);

        }
        [Authorize(Roles = "Driver,Admin")]
        [HttpPatch("acceptRide")]
        public async Task<IActionResult> AcceptRide(string id)
        {
            Rides ride = await _RidesRepository.GetByEmailAsync(id);
            if (ride == null) { return NotFound(); }
            if (ride.Status == "done" || ride.Status == "paid" || ride.Status == "cancelled")
            {
                return BadRequest("Ride is already done");
            }
            ride.Status = "ongoing";
            Driver driver = await _DriverRepository.GetByEmailAsync(ride.DriverEmail!);
            driver.Availability = false;
            await _DriverRepository.UpdateAsync(driver);
            await _RidesRepository.UpdateAsync(ride);
            await _DriverRepository.Save();
            await _RidesRepository.Save();
            Dictionary<String, dynamic> data = new Dictionary<String, dynamic>();
            data.Add("type", "rideUpdated");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            data.Add("data", JsonSerializer.Serialize(ride, options));
            await _HubContext.Clients.All.SendAsync("ridesUpdated", data);
            await _HubContext.Clients.All.SendAsync(ride.PassangerEmail!, ride.Status);
            return Ok(ride);
        }
        [Authorize(Roles = "Driver,Admin")]
        [HttpPatch("endRide")]
        public async Task<IActionResult> EndRide(string id)
        {
            Rides ride = await _RidesRepository.GetByEmailAsync(id);
            if (ride == null) { return NotFound(); }
            if (ride.Status == "done" || ride.Status == "paid" || ride.Status == "cancelled")
            {
                return BadRequest("Ride is already done");
            }
            ride.Status = "done";
            Driver driver = await _DriverRepository.GetByEmailAsync(ride.DriverEmail!);
            driver.Availability = true;
            await _DriverRepository.UpdateAsync(driver);
            await _RidesRepository.UpdateAsync(ride);
            await _DriverRepository.Save();
            await _RidesRepository.Save();
            Dictionary<String, dynamic> data = new Dictionary<String, dynamic>();
            data.Add("type", "rideUpdated");
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };
            data.Add("data", JsonSerializer.Serialize(ride, options));
            await _HubContext.Clients.All.SendAsync(ride.PassangerEmail!, ride.Status);
            await _HubContext.Clients.All.SendAsync("ridesUpdated", data);
            return Ok(ride);
        }
        [Authorize(Roles = "Driver,Admin")]
        [HttpGet("getAllIncomePerDay/{email}")]
        public async Task<IActionResult> GetAllIncomePerDay(string email)
        {
            ICollection<Rides> rides = (await _RidesRepository.GetAllAsync()).Where(r=>r.DriverEmail == email&&r.Status == "paid").ToList();
            if(rides.Count == 0) {  return NotFound(); }
            Dictionary<String, Double> incomeMap = new Dictionary<string, double>();
            foreach (Rides ride in rides)
            {
                if(incomeMap.ContainsKey(ride.Date!)) {
                    incomeMap[ride.Date!] += ride.Price;
                }
                else { incomeMap[ride.Date!] = ride.Price; }
                
            }
            return Ok(incomeMap);
        }
        [Authorize(Roles = "Driver,Admin")]
        [HttpGet("getAllIncomePerMonth/{email}")]
        public async Task<IActionResult> GetAllIncomePerMonth(string email)
        {
            ICollection<Rides> rides = (await _RidesRepository.GetAllAsync()).Where(r => r.DriverEmail == email && r.Status == "paid").ToList();
            if (rides.Count == 0) { return NotFound(); }
            Dictionary<String, Double> incomeMap = new Dictionary<string, double>();
            foreach (Rides ride in rides)
            {
                var splitDate = ride.Date!.Split("-");
                String month = splitDate[0]+'-'+splitDate[1];
                if (incomeMap.ContainsKey(month))
                {
                    incomeMap[month] += ride.Price;
                }
                else { incomeMap[month] = ride.Price; }
            }
            return Ok(incomeMap);
        }

    }
}
