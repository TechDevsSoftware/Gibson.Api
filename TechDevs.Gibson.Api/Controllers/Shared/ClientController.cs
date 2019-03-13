using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.Api.Controllers
{
    [Route("client")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }
        
        [HttpGet]
        public async Task<ActionResult<Client>> GetClient()
        {
            var clientKey = Request.ClientKey();
            var res = await _clientService.GetClientByShortKey(clientKey);
            if (res == null) return new NotFoundResult();
            return new OkObjectResult(res);
        }
    }
}