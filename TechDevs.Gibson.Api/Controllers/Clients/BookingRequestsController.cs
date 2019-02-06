using System.Collections.Generic;
using System.Threading.Tasks;
using Gibson.BookingRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.Api.Controllers
{
    [Authorize]
    [Route("api/v1/clients/data/bookingrequests")]
    public class BookingRequestsController : Controller
    {
        private readonly IBookingRequestService _bookingRequestService;
        private readonly IClientService _clientService;

        public BookingRequestsController(IBookingRequestService bookingRequestService, IClientService clientService)
        {
            _bookingRequestService = bookingRequestService;
            this._clientService = clientService;
        }

        [HttpGet]
        [Produces(typeof(List<BookingRequest>))]
        public async Task<IActionResult> GetBookingRequests()
        {
            var client = await _clientService.GetClientByShortKey(Request.ClientKey());
            return new OkObjectResult(await _bookingRequestService.GetBookings(client.Id));
        }

        [HttpGet("{bookingId}")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> GetBookingRequest([FromRoute] string bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.ClientKey());
            return new OkObjectResult(await _bookingRequestService.GetBooking(bookingId, client.Id));
        }

        [HttpPost]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> CreateBookingRequest([FromBody] BookingRequest_Create bookingRequest)
        {
            return new OkObjectResult(await _bookingRequestService.CreateBooking(bookingRequest, this.UserId().ToString(), this.ClientId().ToString()));
        }

        [HttpPut]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> UpdateBookingRequest([FromBody] BookingRequest bookingRequest)
        {
            var client = await _clientService.GetClientByShortKey(Request.ClientKey());
            return new OkObjectResult(await _bookingRequestService.UpdateBooking(bookingRequest, client.Id));
        }

        [HttpDelete("{bookingId}")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> DeleteBookingRequest([FromRoute] string bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.ClientKey());
            await _bookingRequestService.DeleteBooking(bookingId, client.Id);
            return new OkResult();
        }

        [HttpPost("{bookingId}/confirm")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> ConfirmBooking([FromRoute] string bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.ClientKey());
            await _bookingRequestService.ConfirmBooking(bookingId, client.Id);
            return new OkResult();
        }

        [HttpPost("{bookingId}/cancel")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> CancelBooking([FromRoute] string bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.ClientKey());
            await _bookingRequestService.CancelBooking(bookingId, client.Id);
            return new OkResult();
        }
    }
}