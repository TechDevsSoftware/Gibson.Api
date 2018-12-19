using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Accounts.Services;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/clients")]
    public class ClientThemeController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IClientThemeService _themeService;

        public ClientThemeController(IClientThemeService themeService, IClientService clientService)
        {
            _themeService = themeService;
            _clientService = clientService;
        }

        [HttpPost("{clientKey}/theme/parameters")]
        public async Task<IActionResult> AddParameter([FromRoute]string clientKey, [FromBody]CSSParameter parameter)
        {
            var client = await _clientService.GetClientByShortKey(clientKey);
            if (client == null) return new BadRequestObjectResult("Client could not be found");
            return new OkObjectResult(await _themeService.SetParameter(client.Id, parameter.Key, parameter.Value));
        }

        [HttpDelete("{clientKey}/theme/parameters")]
        public async Task<IActionResult> RemoveParameter([FromRoute]string clientKey, [FromQuery]string key)
        {
            var client = await _clientService.GetClientByShortKey(clientKey);
            if (client == null) return new BadRequestObjectResult("Client could not be found");
            return new OkObjectResult(await _themeService.RemoveParameter(client.Id, key));
        }
    }
}