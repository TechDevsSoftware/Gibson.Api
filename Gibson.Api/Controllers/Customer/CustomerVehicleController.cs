using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gibson.Customers.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;

namespace Gibson.Api.Controllers
{
    [Authorize(Policy="CustomerDataPolicy")]
    [Route("customers/{customerId}/vehicles")]
    public class CustomerVehicleController : Controller
    {
        private readonly ICustomerVehicleService vehicleService;
        private readonly IVehicleDataService vehicleData;
        private readonly IAuthorizationService _authorizationService;

        public CustomerVehicleController(ICustomerVehicleService vehicleService, IVehicleDataService vehicleData,
            IAuthorizationService authorizationService)
        {
            this.vehicleService = vehicleService;
            this.vehicleData = vehicleData;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromRoute] Guid customerId, [FromQuery] string registration)
        {
            try
            {
                var result = await vehicleService.AddVehicleToCustomer(registration, customerId, this.ClientId());
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be added");
            }
        }

        [HttpDelete("{vehicleId}")]
        public async Task<IActionResult> RemoveVehicle([FromRoute] Guid customerId, [FromRoute] Guid vehicleId)
        {
            try
            {
                await vehicleService.DeleteCustomerVehicle(vehicleId, customerId, this.ClientId());
                return new OkResult();
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be removed");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerVehicle>>> GetCustomerVehicles([FromRoute] Guid customerId)
        {
            try
            {
                // Fetch the data
                var result = await vehicleService.GetCustomerVehicles(customerId, this.ClientId());
                return new OkObjectResult(result);
            }
            catch (UnauthorizedAccessException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

        [HttpGet("lookup")]
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
        public async Task<ActionResult<CustomerVehicle>> UpdateServiceData([FromRoute] Guid vehicleId,
            [FromBody] ServiceData serviceData)
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