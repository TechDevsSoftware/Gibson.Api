using System;
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
    [Route("client/bookingrequests")]
    [ApiExplorerSettings(GroupName = "client")]

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
            return new OkObjectResult(await _bookingRequestService.GetBookings(this.ClientId()));
        }

        [HttpGet("{bookingId}")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> GetBookingRequest([FromRoute] Guid bookingId)
        {
            return new OkObjectResult(await _bookingRequestService.GetBooking(bookingId, this.ClientId()));
        }

        [HttpPost]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> CreateBookingRequest([FromBody] BookingRequest_Create bookingRequest)
        {
            var customerId = this.UserId();
            bookingRequest.CustomerId = customerId;
            return new OkObjectResult(await _bookingRequestService.CreateBooking(bookingRequest, this.ClientId()));
        }

        [HttpPut]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> UpdateBookingRequest([FromBody] BookingRequest bookingRequest)
        {
            return new OkObjectResult(await _bookingRequestService.UpdateBooking(bookingRequest, this.ClientId()));
        }

        [HttpDelete("{bookingId}")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> DeleteBookingRequest([FromRoute] Guid bookingId)
        {
            await _bookingRequestService.DeleteBooking(bookingId, this.ClientId());
            return new OkResult();
        }

        [HttpPost("{bookingId}/confirm")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> ConfirmBooking([FromRoute] Guid bookingId)
        {
            await _bookingRequestService.ConfirmBooking(bookingId, this.ClientId());
            return new OkResult();
        }

        [HttpPost("{bookingId}/cancel")]
        [Produces(typeof(BookingRequest))]
        public async Task<IActionResult> CancelBooking([FromRoute] Guid bookingId)
        {
            await _bookingRequestService.CancelBooking(bookingId, this.ClientId());
            return new OkResult();
        }
    }
}