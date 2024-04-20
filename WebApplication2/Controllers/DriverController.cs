﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication2.Dtos;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : Controller
    {
        private readonly IDataRepository<Driver> _DriverRepository;
        private readonly IDataRepository<Credentials> _CredentialsRepository;
        private readonly IDataRepository<Rides> _RidesRepository;
        private readonly IDataRepository<Passanger> _PassangerRepository;
        public DriverController(
             IDataRepository<Driver> DriverRepository,
        IDataRepository<Credentials> CredentialsRepository,
        IDataRepository<Rides> RidesRepository,
        IDataRepository<Passanger> PassangerRepository
            )
        {
            _DriverRepository = DriverRepository;
            _CredentialsRepository = CredentialsRepository;
            _PassangerRepository =  PassangerRepository;
            _RidesRepository = RidesRepository;
        }

       

        [HttpGet("getDriverByEmail/{email}")]
        public async Task<IActionResult> GetDriverByEmail(string email)
        {
            await _PassangerRepository.GetAllAsync();
            Driver driver= await _DriverRepository.GetByEmailAsync(email);
            if (driver == null) { return NotFound("no passanger with this email found"); }
            await _RidesRepository.GetAllAsync();
            foreach (var ride in driver.Rides!)
            {
                ride.Passanger!.Rides = null;
            }
            return Ok(driver);

        }
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
        public async Task<IActionResult> CreateDriver(NewDriverDto newDriver)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (newDriver.Password != newDriver.ConfirmPassword)
            {
                return BadRequest("Password doesn't match");
            }


            Credentials credentials = new Credentials()
            {
                Email = newDriver.Email,
                Password = newDriver.Password,
                Role = newDriver.Role
            };
            if ((await _CredentialsRepository.GetByEmailAsync(credentials.Email!)) != null)
            {
                return BadRequest("Email already exists");
            }
            Driver driver = new Driver()
            {
                Email = newDriver.Email,
                Gender = newDriver.Gender,
                CarType = newDriver.CarType,
                City = newDriver.City,
                Region = newDriver.Region,
                Smoking = newDriver.Smoking,
                Username = newDriver.Username
            };

            await _CredentialsRepository.AddAsync(credentials);
            await _CredentialsRepository.Save();
            await _DriverRepository.AddAsync(driver);
            await _DriverRepository.Save();


            return Ok(driver);
        }

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
            return Ok(driver);
        }
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
            return Ok(ride);

        }
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

            return Ok(ride);
        }
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

            return Ok(ride);
        }
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
