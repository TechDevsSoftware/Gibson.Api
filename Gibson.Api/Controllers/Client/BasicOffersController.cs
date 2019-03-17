using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gibson.Clients;
using Gibson.Clients.Offers;
using Gibson.Common.Models;

namespace Gibson.Api.Controllers
{
    [ApiExplorerSettings(GroupName = "client")]
    [Route("client/basicoffers")]
    public class BasicOffersController : Controller
    {
        private readonly IBasicOffersService _basicOffersService;
        private readonly IClientService _clientService;

        public BasicOffersController(IBasicOffersService _basicOffersService, IClientService clientService)
        {
            this._basicOffersService = _basicOffersService;
            this._clientService = clientService;
        }

        [HttpPost]
        [Produces(typeof(Client))]
        public async Task<IActionResult> UpdateBasicOffer([FromBody] BasicOffer offer)
        {
            var client = await _clientService.GetClientByShortKey(Request.ClientKey());
            return new OkObjectResult(await _basicOffersService.UpdateBasicOffer(offer, client.Id));
        }

        [HttpDelete("{offerId}")]
        [Produces(typeof(Client))]
        public async Task<IActionResult> DeleteBasicOffer([FromRoute] string offerId)
        {
            var client = await _clientService.GetClientByShortKey(Request.ClientKey());
            return new OkObjectResult(await _basicOffersService.DeleteBasicOffer(offerId, client.Id));
        }
    }
}