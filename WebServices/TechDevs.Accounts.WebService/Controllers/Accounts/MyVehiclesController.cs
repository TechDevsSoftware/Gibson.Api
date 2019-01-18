using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.MyVehicles;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.WebService.Controllers
{
    [Route("api/v1/account/myvehicles")]
    [Authorize]
    public class MyVehiclesController : Controller
    {
        private readonly IMyVehicleService _myVehicleService;
        private readonly IClientService _clientService;

        public MyVehiclesController(IMyVehicleService myVehicleService, IClientService clientService)
        {
            _myVehicleService = myVehicleService;
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] CustomerVehicle vehicle)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(Request.GetClientKey());

                var userId = this.UserId();
                if (userId == null) return new UnauthorizedResult();

                var result = await _myVehicleService.AddVehicle(vehicle, userId, client.Id);
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
                var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
                var userId = this.UserId();
                if (userId == null) return new UnauthorizedResult();

                var result = await _myVehicleService.RemoveVehicle(registration, userId, client.Id);
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be removed");
            }
        }

        [HttpGet]
        [Route("lookup")]
        [AllowAnonymous]
        public async Task<IActionResult> LookupVehicle(string registration)
        {
            var result = await _myVehicleService.LookupVehicle(registration);
            return new OkObjectResult(result);
        }

        [HttpPost("refreshmot")]
        public async Task<IActionResult> UpdateVehicleMOTData(string registration)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
                var userId = this.UserId();
                if (userId == null) return new UnauthorizedResult();

                var result = await _myVehicleService.UpdateVehicleMOTData(registration, userId, client.Id);
                return new OkObjectResult(result);
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}