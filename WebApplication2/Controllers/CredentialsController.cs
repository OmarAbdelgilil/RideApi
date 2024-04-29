using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using WebApplication1.Data;
using WebApplication2.Auth;
using WebApplication2.Dtos;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api")]
    [ApiController]
    public class CredentialsController : Controller
    {
        private readonly IDataRepository<Passanger> _PassangerRepository;
        private readonly IDataRepository<Credentials> _CredentialsRepository;
        private readonly IDataRepository<Driver> _DriverRepository;
        private readonly AuthInterface _auth;
        public CredentialsController(
             IDataRepository<Passanger> PassangerRepository,
             IDataRepository<Credentials> CredentialsRepository,
             IDataRepository<Driver> DriverRepository,
            AuthInterface auth
            )
        {
            _PassangerRepository = PassangerRepository;
            _CredentialsRepository = CredentialsRepository;
            _DriverRepository = DriverRepository;
            _auth = auth;

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Credentials c =await  _CredentialsRepository.GetByEmailAsync(login.Email!);
            if(c == null) { return NotFound("Email or password are in correct"); }

            if(!_auth.VerifyPasswordHash(login.Password!,c.PasswordHash!,c.PasswordSalt!))
            {
                return NotFound("Email or password are in correct");
            }

            String token = _auth.LoginGetToken(login.Email!, login.Password!, c.Role!);
            if (c.Role == "Passenger")
            {
                if (c.Registered == 1)
                {
                    return BadRequest("Account pending");
                }
                if (c.Registered == -1)
                {
                    return BadRequest("Account rejected");
                }
                if (c.Registered != 2)
                {
                    return BadRequest();
                }
                Passanger passanger = await _PassangerRepository.GetByEmailAsync(login.Email!);
                Dictionary<string, dynamic> map = new Dictionary<string, dynamic>();
                map.Add("role",c.Role);
                map.Add("data",passanger);
                return Ok(new {Token = token });
            }
            if (c.Role == "Driver")
            {
                if (c.Registered == 1)
                {
                    return BadRequest("Account pending");
                }
                if (c.Registered == -1)
                {
                    return BadRequest("Account rejected");
                }
                if (c.Registered != 2)
                {
                    return BadRequest();
                }
                Driver driver = await _DriverRepository.GetByEmailAsync(login.Email!);
                if(driver.Blocked == true)
                {
                    return BadRequest("Account Blocked by admin");
                }
                Dictionary<string, dynamic> map = new Dictionary<string, dynamic>();
                map.Add("role", c.Role);
                map.Add("data", driver);
                return Ok(new { Token = token });
            }
            if(c.Role == "Admin")
            {
                Admin admin = new Admin() { Email = login.Email };
                return Ok(new { Token = token });
            }

            return BadRequest();
        }
    }
}
