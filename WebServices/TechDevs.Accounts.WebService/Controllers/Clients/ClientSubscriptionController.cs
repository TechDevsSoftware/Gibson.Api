using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService
{
    [Route("api/v1/clients/{clientId}/subscription")]
    public class ClientSubscriptionController: Controller
    {
        private readonly IClientSubscriptionService _service;
        private readonly IClientService _clientService;

        public ClientSubscriptionController(IClientSubscriptionService service, IClientService clientService)
        {
            this._service = service;
            this._clientService = clientService;
        }

        [HttpPost("setPackage")]
        public async Task<IActionResult> SetCurrentPackage([FromBody] Product product)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var result = await _service.SetCurrentPackage(product, client.Id);
            return new OkObjectResult(result);
        }

        [HttpPost("setStatus")]
        public async Task<IActionResult> SetSubscriptionStatus([FromQuery] SubscriptionStatus status)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var result = await _service.SetSubscriptionStatus(status, client.Id);
            return new OkObjectResult(result);
        }
    }
}