using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Clients.BookingRequests;
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
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            return new OkObjectResult(await _bookingRequestService.GetBookings(client.Id));
        }

        [HttpGet("{bookingId}")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> GetBookingRequest([FromRoute] string bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            return new OkObjectResult(await _bookingRequestService.GetBooking(bookingId, client.Id));
        }

        [HttpPost]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> CreateBookingRequest([FromBody] BookingRequest_Create bookingRequest)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            return new OkObjectResult(await _bookingRequestService.CreateBooking(bookingRequest, userId, client.Id));
        }

        [HttpPut]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> UpdateBookingRequest([FromBody] BookingRequest bookingRequest)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            return new OkObjectResult(await _bookingRequestService.UpdateBooking(bookingRequest, client.Id));
        }

        [HttpDelete("{bookingId}")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> DeleteBookingRequest([FromRoute] string bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            await _bookingRequestService.DeleteBooking(bookingId, client.Id);
            return new OkResult();
        }

        [HttpPost("{bookingId}/confirm")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> ConfirmBooking([FromRoute] string bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            await _bookingRequestService.ConfirmBooking(bookingId, client.Id);
            return new OkResult();
        }

        [HttpPost("{bookingId}/cancel")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> CancelBooking([FromRoute] string bookingId)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            await _bookingRequestService.CancelBooking(bookingId, client.Id);
            return new OkResult();
        }
    }
}