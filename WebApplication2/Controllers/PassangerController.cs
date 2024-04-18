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
        public async Task<IActionResult> getAllPassengers()
        {
            var passangers = (await  _PassangerRepository.GetAllAsync()).ToList();
            if (passangers==null || !passangers.Any())
            {
                return NotFound("No passangers found");
            }
            return Ok(passangers);
            }

        [HttpGet("getPassengerByEmail")]
        public async Task<IActionResult> getPassengerByEmail(string email)
        {
            Passanger passanger = await _PassangerRepository.GetByEmailAsync(email);
            if (passanger==null) { return NotFound("no passanger with this eamil found"); }
            return Ok(passanger);

        }

        [HttpPost("createPassenger")]
        public async Task<IActionResult> createPassenger(NewPassengerDto newPassanger)
        {
            if (!ModelState.IsValid)
                {
                return BadRequest();
            }
            if(newPassanger.Password != newPassanger.ConfirmPassword)
            {
                return BadRequest("Password doesn't match");
            }

           
            Credentials credentials = new Credentials()
            {
                Email = newPassanger.Email,
                Password = newPassanger.Password,
                Role = newPassanger.Role
            };
             
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

    }
    
}
