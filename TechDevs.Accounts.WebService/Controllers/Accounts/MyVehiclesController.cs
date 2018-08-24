using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Accounts.ExtMethods;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Route("api/v1/account/myvehicles")]
    [Authorize]
    public class MyVehiclesController : Controller
    {
        private readonly IMyVehicleService _myVehicleService;

        public MyVehiclesController(IMyVehicleService myVehicleService)
        {
            _myVehicleService = myVehicleService;
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] CustomerVehicle vehicle)
        {
            try
            {
                var userId = this.UserId();
                if (userId == null) return new UnauthorizedResult();

                var result = await _myVehicleService.AddVehicle(vehicle, userId);
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be added");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveVehicle(string registration)
        {
            try
            {
                var userId = this.UserId();
                if (userId == null) return new UnauthorizedResult();

                var result = await _myVehicleService.RemoveVehicle(registration, userId);
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be removed");
            }
        }

        [HttpGet]
        [Route("lookup")]
        public async Task<IActionResult> LookupVehicle(string registration)
        {
            var result = await _myVehicleService.LookupVehicle(registration);
            return new OkObjectResult(result);
        }
    }
}