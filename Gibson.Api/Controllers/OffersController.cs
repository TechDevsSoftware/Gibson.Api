using System;
using System.Collections.Generic;
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

        [HttpGet("active")]
        [Authorize]
        public async Task<ActionResult<List<BasicOffer>>> GetActiveOffers([FromRoute] Guid clientId)
        {
            try
            {
                var result = await _offersService.GetActiveOffers(clientId);
                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<Client>> UpdateOffer([FromBody] BasicOffer offer, [FromRoute] Guid clientId)
        {
            try
            {
                var result = await _offersService.CreateOffer(offer, clientId);
                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

        [HttpPut]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<BasicOffer>> CreateOffer([FromBody] BasicOffer offer, [FromRoute] Guid clientId)
        {
            try
            {
                var result = await _offersService.CreateOffer(offer, clientId);
                return result;
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

        [HttpDelete("{offerId}")]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult> DeleteOffer([FromRoute] Guid offerId, [FromRoute] Guid clientId)
        {
            try
            {
                await _offersService.DeleteOffer(offerId, clientId);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }
    }
}