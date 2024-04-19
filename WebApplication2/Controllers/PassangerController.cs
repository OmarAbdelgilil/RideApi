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


        public PassangerController(
             IDataRepository<Passanger> PassangerRepository,
        IDataRepository<Credentials> CredentialsRepository
            )
        {
            _PassangerRepository = PassangerRepository;
            _CredentialsRepository = CredentialsRepository;

        }

        [HttpGet("getAllPassengers")]
        public async Task<IActionResult> GetAllPassengers()
        {
            var passangers = (await _PassangerRepository.GetAllAsync()).ToList();
            if (passangers == null || !passangers.Any())
            {
                return NotFound("No passangers found");
            }
            return Ok(passangers);
        }

        [HttpGet("getPassengerByEmail")]
        public async Task<IActionResult> GetPassengerByEmail(string email)
        {
            Passanger passanger = await _PassangerRepository.GetByEmailAsync(email);
            if (passanger == null) { return NotFound("no passanger with this eamil found"); }
            return Ok(passanger);

        }
        [HttpDelete("deletePassenger")]
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
        
    }
}
