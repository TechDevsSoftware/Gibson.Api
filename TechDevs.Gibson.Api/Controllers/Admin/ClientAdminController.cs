using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.Api.Controllers
{
    [Route("clients")]
    [ApiExplorerSettings(GroupName = "admin")]

    public class ClientAdminController : Controller
    {
        private readonly IClientService _clientService;

        public ClientAdminController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        [Produces(typeof(List<Client>))]
        public async Task<IActionResult> GetClients() =>
            new OkObjectResult(await _clientService.GetClients());

        [HttpGet("{clientId}")]
        [Produces(typeof(Client))]
        public async Task<IActionResult> GetClientById([FromRoute] string clientId) =>
            new OkObjectResult(await _clientService.GetClient(clientId));

        [HttpGet("customer")]
        [Produces(typeof(List<PublicClient>))]
        public async Task<IActionResult> GetClientsByCustomerEmail([FromQuery] string customerEmail) =>
            new OkObjectResult(await _clientService.GetClientsByCustomer(customerEmail));

        [HttpPost]
        [Produces(typeof(Client))]
        public async Task<IActionResult> CreateClient([FromBody] ClientRegistration client)
            => new OkObjectResult(await _clientService.CreateClient(client));

        [HttpPut("{clientId}")]
        [Produces(typeof(Client))]
        public async Task<IActionResult> UpdateClient(string clientId, [FromBody] Client client) =>
            new OkObjectResult(await _clientService.UpdateClient(clientId, client));

        [HttpDelete("{clientId}")]
        [Produces(typeof(Client))]
        public async Task<IActionResult> DeleteClient([FromRoute] string clientId) =>
            new OkObjectResult(await _clientService.DeleteClient(clientId));
    }
}