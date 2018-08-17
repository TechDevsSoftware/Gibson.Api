using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        [Route("{clientId}")]
        public async Task<IActionResult> GetClient([FromRoute] string clientId)
        {
            return new OkObjectResult(await _clientService.GetClient(clientId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] IClient client)
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