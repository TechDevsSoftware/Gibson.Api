using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Accounts.Services;

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

        [HttpGet]
        [Route("{clientId}")]
        public async Task<IActionResult> GetClient([FromRoute] string clientId, [FromQuery] bool includeRelatedAuthUsers = false)
        {
            return new OkObjectResult(await _clientService.GetClient(clientId, includeRelatedAuthUsers));
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientRegistration client)
        {
            return new OkObjectResult(await _clientService.CreateClient(client));
        }

        // Not yet supported
        //[HttpPut]
        //public async Task<IActionResult> UpdateClient(string clientId)
        //{
        //    return new OkObjectResult(await _clientService.UpdateClient(""));
        //}

        [HttpDelete]
        [Route("{clientId}")]
        public async Task<IActionResult> DeleteClient([FromRoute] string clientId)
        {
            return new OkObjectResult(await _clientService.DeleteClient(clientId));
        }
    }
}