using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;
using Microsoft.AspNetCore.Authorization;
using TechDevs.Clients;

namespace Gibson.Api.Controllers
{
    [Route("clients/{clientId}")]
    public class ClientThemeController : Controller
    {
        private readonly IClientThemeService _themeService;

        public ClientThemeController(IClientThemeService themeService)
        {
            _themeService = themeService;
        }

        [HttpPost("theme/parameters")]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<Client>> AddParameter([FromBody]CSSParameter parameter, [FromRoute] Guid clientId)
        {
            var result = await _themeService.SetParameter(clientId.ToString(), parameter.Key, parameter.Value);
            return new OkObjectResult(result);
        }

        [HttpDelete("theme/parameters")]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<Client>> RemoveParameter([FromQuery]string key, [FromRoute] Guid clientId)
        {
            var result = await _themeService.RemoveParameter(clientId.ToString(), key);
            return new OkObjectResult(result);
        }
    }
}