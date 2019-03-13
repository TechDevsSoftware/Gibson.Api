using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Gibson.CustomerVehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.Api.Controllers
{
    
    [ApiExplorerSettings(GroupName = "customer")]
    [Route("user/vehicles")]
    [Authorize]
    public class UserVehiclesController : Controller
    {
        private readonly ICustomerVehicleService vehicleService;
        private readonly IVehicleDataService vehicleData;

        public UserVehiclesController(ICustomerVehicleService vehicleService, IVehicleDataService vehicleData )
        {
            this.vehicleService = vehicleService;
            this.vehicleData = vehicleData;
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromQuery] string registration)
        {
            try
            {
                var result = await vehicleService.AddVehicleToCustomer(registration, this.UserId(), this.ClientId());
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be added");
            }
        }

        [HttpDelete("{vehicleId}")]
        public async Task<IActionResult> RemoveVehicle([FromRoute] Guid vehicleId)
        {
            try
            {
                await vehicleService.DeleteCustomerVehicle(vehicleId, this.UserId(), this.ClientId());
                return new OkResult();
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
            var result = await vehicleData.GetVehicleData(registration);
            return new OkObjectResult(result);
        }

        [HttpPost("{vehicleId}/refreshmot")]
        public async Task<IActionResult> UpdateVehicleMOTData([FromRoute] Guid vehicleId)
        {
            try
            {
                var result = await vehicleService.UpdateMotData(vehicleId, this.ClientId());
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message); 
                throw;
            }
        }

        [HttpPost("{vehicleId}/servicedata")]
        public async Task<ActionResult<CustomerVehicle>> UpdateServiceData([FromRoute] Guid vehicleId, [FromBody] ServiceData serviceData)
        {
            try
            {
                var result = await vehicleService.UpdateServiceData(serviceData, vehicleId, this.ClientId());
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message); 
                throw;
            }
        }
    }
}