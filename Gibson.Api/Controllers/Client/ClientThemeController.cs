using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gibson.Clients;
using Gibson.Common.Models;
using TechDevs.Clients;

namespace Gibson.Api.Controllers
{
    [Produces("application/json")]
    [Route("client")]
    [ApiExplorerSettings(GroupName = "client")]
    public class ClientThemeController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IClientThemeService _themeService;

        public ClientThemeController(IClientThemeService themeService, IClientService clientService)
        {
            _themeService = themeService;
            _clientService = clientService;
        }

        [HttpPost("theme/parameters")]
        [Produces(typeof(Client))]
        public async Task<IActionResult> AddParameter([FromBody]CSSParameter parameter)
        {
            var clientKey = this.GetClientKey();
            var client = await _clientService.GetClientByShortKey(clientKey);
            if (client == null) return new BadRequestObjectResult("Client could not be found");
            return new OkObjectResult(await _themeService.SetParameter(client.Id, parameter.Key, parameter.Value));
        }

        [HttpDelete("theme/parameters")]
        [Produces(typeof(Client))]
        public async Task<IActionResult> RemoveParameter([FromQuery]string key)
        {
            var clientKey = this.GetClientKey();
            var client = await _clientService.GetClientByShortKey(clientKey);
            if (client == null) return new BadRequestObjectResult("Client could not be found");
            return new OkObjectResult(await _themeService.RemoveParameter(client.Id, key));
        }
    }
}