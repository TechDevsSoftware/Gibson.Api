using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Gibson.Clients;
using Gibson.Clients.Offers;
using Gibson.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace Gibson.Api.Controllers
{
    [Route("clients/{clientId}/offers")]
    public class OffersController : Controller
    {
        private readonly IOffersService _offersService;

        public OffersController(IOffersService offersService)
        {
            _offersService = offersService;
        }

        [HttpPost]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<Client>> UpdateOffer([FromBody] BasicOffer offer, [FromRoute] Guid clientId)
        {
            try
            {
                var result = await _offersService.UpdateBasicOffer(offer, clientId.ToString());
                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

        [HttpDelete("{offerId}")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult> DeleteOffer([FromRoute] string offerId, [FromRoute] Guid clientId)
        {
            try
            {
                await _offersService.DeleteBasicOffer(offerId, clientId.ToString());
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }
    }
}