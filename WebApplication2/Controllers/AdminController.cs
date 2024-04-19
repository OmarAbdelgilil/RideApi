using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication2.Models;

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
        public AdminController(IDataRepository<Rides> RidesRepository,IDataRepository<Driver> DriverRepository, IDataRepository<Credentials> CredentialsRepository, IDataRepository<Passanger> PassangerRepository) { _CredentialsRepository = CredentialsRepository; _PassangerRepository = PassangerRepository; _DriverRepository = DriverRepository;  _RidesRepository = RidesRepository; }

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
        [HttpPost("blockDriver")]
        public async Task<IActionResult> BlockDriver(String email)
        {
            Driver driver = await _DriverRepository.GetByEmailAsync(email);
            if (driver == null) { return NotFound("driver not found"); }
            driver.Blocked = true;
            driver.Availability = false;
            await _DriverRepository.UpdateAsync(driver);
            await _DriverRepository.Save();
            return Ok();
        }
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
        [HttpGet("getAllRides")]
        public async Task<IActionResult> GetAllRides()
        {
            return Ok(await _RidesRepository.GetAllAsync());
        }
    }
}
