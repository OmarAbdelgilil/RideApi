using Microsoft.AspNetCore.Http;
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

        public DriverController(
             IDataRepository<Driver> DriverRepository,
        IDataRepository<Credentials> CredentialsRepository
            )
        {
            _DriverRepository = DriverRepository;
            _CredentialsRepository = CredentialsRepository;
        }

        [HttpGet("getAllDriver")]
        public async Task<IActionResult> GetAllDriver()
        {
            var driver = (await _DriverRepository.GetAllAsync()).ToList();
            if (driver == null || driver.Count == 0)
            {
                return NotFound("No drivers found");
            }
            return Ok(driver);
        }

        [HttpGet("getDriverByEmail/{email}")]
        public async Task<IActionResult> GetDriverByEmail(string email)
        {
            Driver driver= await _DriverRepository.GetByEmailAsync(email);
            if (driver == null) { return NotFound("no passanger with this eamil found"); }
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
        public async Task<IActionResult> ChangePassword(String email, String old, String newPassword)
        {
            Credentials? driver = await _CredentialsRepository.GetByEmailAsync(email);
            if (driver == null)
            {
                return NotFound("No Drivers found");
            }
            if (driver.Password == old)
            {
                driver.Password = newPassword;
                await _CredentialsRepository.UpdateAsync(driver);
                await _CredentialsRepository.Save();
                return Ok();
            }
            else
            {
                return BadRequest("old password is wrong");
            }
        }

        [HttpPost("updateDriver")]
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
    }
}
