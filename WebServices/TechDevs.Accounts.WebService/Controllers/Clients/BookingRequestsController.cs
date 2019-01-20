using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Clients.BookingRequests;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.WebService.Controllers
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

        [HttpPost]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> CreateBookingRequest([FromBody] BookingRequest bookingRequest)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            bookingRequest.CustomerId = new DBRef { Id = userId };
            return new OkObjectResult(await _bookingRequestService.CreateBookingRequest(bookingRequest, client.Id));
        }

        [HttpPut]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> UpdateBookingRequest([FromBody] BookingRequest bookingRequest)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            return new OkObjectResult(await _bookingRequestService.UpdateBookingRequest(bookingRequest, client.Id));
        }

        [HttpDelete]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> DeleteBookingRequest([FromBody] Guid bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            await _bookingRequestService.DeleteBookingRequest(bookingId, client.Id);
            return new OkResult();
        }

        [HttpPost("confirm")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> ConfirmBooking(Guid bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            await _bookingRequestService.ConfirmBooking(bookingId, client.Id);
            return new OkResult();
        }

        [HttpPost("cancel")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            await _bookingRequestService.ConfirmBooking(bookingId, client.Id);
            return new OkResult();
        }

    }
}