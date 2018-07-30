using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Accounts.ExtMethods;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Route("api/v1/account/myvehicles")]
    public class MyVehiclesController : Controller
    {
        private readonly IMyVehicleService _myVehicleService;

        public MyVehiclesController(IMyVehicleService myVehicleService)
        {
            _myVehicleService = myVehicleService;
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] UserVehicle vehicle)
        {
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var result = await _myVehicleService.AddVehicle(vehicle, userId);
            return new OkObjectResult(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveVehicle(string registration)
        {
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var result = await _myVehicleService.RemoveVehicle(registration, userId);
            return new OkObjectResult(result);
        }

    }
}