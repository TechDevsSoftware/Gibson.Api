using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService.Controllers
{

    [Produces("application/json")]
    [Route("api/v1/clients")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetClients()
        {
            return new OkObjectResult(await _clientService.GetClients());
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentClient([FromHeader(Name = "TechDevs-ClientKey")] string clientKey)
        {
            return new OkObjectResult(await _clientService.GetClientByShortKey(clientKey));
        }

        [HttpGet]
        [Route("{clientId}")]
        public async Task<IActionResult> GetClientById([FromRoute] string clientId, [FromQuery] bool includeRelatedAuthUsers = false)
        {
            return new OkObjectResult(await _clientService.GetClient(clientId, includeRelatedAuthUsers));
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientRegistration client)
        {
            return new OkObjectResult(await _clientService.CreateClient(client));
        }

        [HttpPut("{clientId}")]
        public async Task<IActionResult> UpdateClient(string clientId, [FromBody] ClientUpdate update)
        {
            var result = await _clientService.UpdateClient<string>(update.PropertyPath, update.UpdateValue, clientId);
            return new OkObjectResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClient(string clientId, [FromBody] Client client)
        {
            return new OkObjectResult(await _clientService.UpdateClient(clientId, client));
        }

        [HttpDelete]
        [Route("{clientId}")]
        public async Task<IActionResult> DeleteClient([FromRoute] string clientId)
        {
            return new OkObjectResult(await _clientService.DeleteClient(clientId));
        }
    }

    public class ClientUpdate
    {
        public string PropertyPath { get; set; }
        public string UpdateValue { get; set; }
    }
}