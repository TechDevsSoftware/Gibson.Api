using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gibson.Customers.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;

namespace Gibson.Api.Controllers
{
    [Route("clients/{clientId}/customers/{customerId}/vehicles")]
    public class CustomerVehicleController : Controller
    {
        private readonly ICustomerVehicleService _vehicleService;
        private readonly IVehicleDataService _vehicleData;
        private readonly IAuthorizationService _authorizationService;

        public CustomerVehicleController(ICustomerVehicleService vehicleService, IVehicleDataService vehicleData,
            IAuthorizationService authorizationService)
        {
            _vehicleService = vehicleService;
            _vehicleData = vehicleData;
            _authorizationService = authorizationService;
        }

        [HttpPost]
        [Authorize(Policy="CustomerData")]
        public async Task<IActionResult> AddVehicle([FromRoute] Guid customerId, [FromQuery] string registration, [FromRoute] Guid clientId)
        {
            try
            {
                var result = await _vehicleService.AddVehicleToCustomer(registration, customerId, clientId);
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be added");
            }
        }

        [HttpDelete("{vehicleId}")]
        [Authorize(Policy="CustomerData")]
        public async Task<IActionResult> RemoveVehicle([FromRoute] Guid customerId, [FromRoute] Guid vehicleId, [FromRoute] Guid clientId)
        {
            try
            {
                await _vehicleService.DeleteCustomerVehicle(vehicleId, customerId, clientId);
                return new OkResult();
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be removed");
            }
        }

        [HttpGet]
        [Authorize(Policy="CustomerData")]
        public async Task<ActionResult<List<CustomerVehicle>>> GetCustomerVehicles([FromRoute] Guid customerId, [FromRoute] Guid clientId)
        {
            try
            {
                // Fetch the data
                var result = await _vehicleService.GetCustomerVehicles(customerId, clientId);
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
        [Authorize(Policy="CustomerData")]
        public async Task<IActionResult> LookupVehicle(string registration)
        {
            var result = await _vehicleData.GetVehicleData(registration);
            return new OkObjectResult(result);
        }

        [HttpPost("{vehicleId}/refreshmot")]
        [Authorize(Policy="CustomerData")]
        public async Task<IActionResult> UpdateVehicleMotData([FromRoute] Guid vehicleId, [FromRoute] Guid clientId)
        {
            try
            {
                var result = await _vehicleService.UpdateMotData(vehicleId, clientId);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
                throw;
            }
        }

        [HttpPost("{vehicleId}/refreshservice")]
        [Authorize(Policy="CustomerData")]
        public async Task<ActionResult<CustomerVehicle>> UpdateServiceData([FromRoute] Guid vehicleId,
            [FromBody] ServiceData serviceData, [FromRoute] Guid clientId)
        {
            try
            {
                var result = await _vehicleService.UpdateServiceData(serviceData, vehicleId, clientId);
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