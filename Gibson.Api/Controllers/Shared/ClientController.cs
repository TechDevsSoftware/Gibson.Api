using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gibson.Clients;
using Gibson.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace Gibson.Api.Controllers
{
    [Route("client")]
    [AllowAnonymous]
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
            var clientId = User.ClientId();
            var res = await _clientService.GetClient(clientId.ToString());
            if (res == null) return new NotFoundResult();
            return new OkObjectResult(res);
        }
    }
}