using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication2.Auth;
using WebApplication2.Migrations;
using WebApplication2.Models;
using WebApplication2.RealTime;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IDataRepository<Credentials> _CredentialsRepository;
        private readonly IDataRepository<Passanger> _PassangerRepository;
        private readonly IDataRepository<Driver> _DriverRepository;
        private readonly IDataRepository<Rides> _RidesRepository;
        private readonly IHubContext<SignalRHub> _HubContext;
        private readonly AuthInterface _auth;
        public AdminController(AuthInterface auth, IDataRepository<Rides> RidesRepository, IDataRepository<Driver> DriverRepository, IDataRepository<Credentials> CredentialsRepository, IDataRepository<Passanger> PassangerRepository, IHubContext<SignalRHub> hubContext) { _CredentialsRepository = CredentialsRepository; _auth = auth; _PassangerRepository = PassangerRepository; _DriverRepository = DriverRepository; _RidesRepository = RidesRepository; _HubContext = hubContext; }


        [HttpPost("createAdmin")]
        public async Task<IActionResult> CreateAdmin(String email, String password)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!await (_auth.Register(email, password, "Admin")))
            {
                return BadRequest("Email already exists");
            }
            return Ok("Admin Created Successfully");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getallPending")]
        public async Task<IActionResult> GetAllPending()
        {
            ICollection<Credentials> credentials = (await _CredentialsRepository.GetAllAsync()).Where(e => e.Registered == 1).ToList();
            ICollection<Dictionary<string,dynamic>> UsersPending = new HashSet<Dictionary<string,dynamic>>();
            foreach(Credentials c in credentials)
            {
                if(c.Role == "Passenger")
                {

                    Dictionary<string,dynamic> keyValuePairs = new Dictionary<string,dynamic>();
                    keyValuePairs.Add("role", c.Role);
                    keyValuePairs.Add("data", await _PassangerRepository.GetByEmailAsync(c.Email!));
                    UsersPending.Add(keyValuePairs);
                }
                if (c.Role == "Driver")
                {

                    Dictionary<string, dynamic> keyValuePairs = new Dictionary<string, dynamic>();
                    keyValuePairs.Add("role", c.Role);
                    keyValuePairs.Add("data", await _DriverRepository.GetByEmailAsync(c.Email!));
                    UsersPending.Add(keyValuePairs);
                }
            }
            if(UsersPending.Count == 0)
            {
                return NotFound("No users are pending");
            }
            return Ok(UsersPending);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("acceptAccount")]
        public async Task<IActionResult> AcceptAccount(String email)
        {
            Credentials credentials = await _CredentialsRepository.GetByEmailAsync(email);

            if (credentials == null) { return NotFound("No user found"); }

            credentials.Registered = 2;
            await _CredentialsRepository.UpdateAsync(credentials);
            await _CredentialsRepository.Save();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("rejectAccount")]
        public async Task<IActionResult> RejectAccount(String email)
        {
            Credentials credentials = await _CredentialsRepository.GetByEmailAsync(email);

            if (credentials == null) { return NotFound("No user found"); }

            credentials.Registered = -1;
            await _CredentialsRepository.UpdateAsync(credentials);
            await _CredentialsRepository.Save();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("blockDriver")]
        public async Task<IActionResult> BlockDriver(String email)
        {
            Driver driver = await _DriverRepository.GetByEmailAsync(email);
            if (driver == null) { return NotFound("driver not found"); }
            driver.Blocked = true;
            driver.Availability = false;
            await _DriverRepository.UpdateAsync(driver);
            await _DriverRepository.Save();
            Dictionary<String,dynamic> data = new Dictionary<String,dynamic>();
            data.Add("type", "block");
            data.Add("data", "");
            await _HubContext.Clients.All.SendAsync(email, JsonSerializer.Serialize(data));
            
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("unBlockDriver")]
        public async Task<IActionResult> UnBlockDriver(String email)
        {
            Driver driver = await _DriverRepository.GetByEmailAsync(email);
            if (driver == null) { return NotFound("driver not found"); }
            driver.Blocked = false;
            await _DriverRepository.UpdateAsync(driver);
            await _DriverRepository.Save();
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("getAllRides")]
        public async Task<IActionResult> GetAllRides()
        {
            await _DriverRepository.GetAllAsync();
            await _PassangerRepository.GetAllAsync();
            ICollection<Rides> rides = (await _RidesRepository.GetAllAsync()).ToList();
            foreach (var ride in rides)
            {
                ride.Passanger!.Rides = null;
                ride.Driver!.Rides = null;
            }
            return Ok(rides);
        }
        [Authorize(Roles = "Admin,Passenger")]
        [HttpGet("getAllDriver")]
        public async Task<IActionResult> GetAllDriver()
        {
            await _PassangerRepository.GetAllAsync();
            var drivers = (await _DriverRepository.GetAllAsync()).ToList();
            if (drivers == null || drivers.Count == 0)
            {
                return NotFound("No drivers found");
            }
            await _RidesRepository.GetAllAsync();
            foreach (Driver driver in drivers)
            {
                if (driver.Rides == null) continue;
                foreach (Rides ride in driver.Rides!)
                {
                    ride.Passanger!.Rides = null;
                }
            }
            
            return Ok(drivers);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("getAllPassengers")]
        public async Task<IActionResult> GetAllPassengers()
        {
            await _DriverRepository.GetAllAsync();
            var passangers = (await _PassangerRepository.GetAllAsync()).ToList();
            if (passangers == null || !passangers.Any())
            {
                return NotFound("No passangers found");
            }
            await _RidesRepository.GetAllAsync();
            foreach (Passanger passanger in passangers)
            {
                if (passanger.Rides == null) continue;
                foreach (Rides ride in passanger.Rides)
                {
                    ride.Driver!.Rides = null;
                }
            }
            return Ok(passangers);
        }
    }
}
