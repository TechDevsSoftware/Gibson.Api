using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gibson.Clients;
using Gibson.Common.Models;
using Gibson.Customers.Bookings;

namespace Gibson.Api.Controllers
{
    
    [Route("clients/{clientId}/customers/{customerId}/bookings")]
    public class CustomerBookingsController : Controller
    {
        private readonly IBookingRequestService _bookingRequestService;

        public CustomerBookingsController(IBookingRequestService bookingRequestService)
        {
            _bookingRequestService = bookingRequestService;
        }
        
        [HttpGet("~/clients/{clientId}/bookings")]
        [Authorize(Policy = "ClientDataPolicy")]
        public async Task<ActionResult<List<BookingRequest>>> GetClientBookings([FromRoute] Guid clientId)
        {
            return new OkObjectResult(await _bookingRequestService.GetBookings(clientId));
        }
        
        [HttpGet()]
        [Authorize(Policy = "CustomerDataPolicy")]
        public async Task<ActionResult<List<BookingRequest>>> GetCustomerBookings([FromRoute] Guid customerId, [FromRoute] Guid clientId)
        {
            return new OkObjectResult(await _bookingRequestService.GetBookingsByCustomer(customerId, clientId));
        }

        [HttpGet("{bookingId}")]
        [Authorize(Policy = "CustomerDataPolicy")]
        public async Task<ActionResult<BookingRequest>> GetBookingRequest([FromRoute] Guid bookingId, [FromRoute] Guid clientId)
        {
            return new OkObjectResult(await _bookingRequestService.GetBooking(bookingId, clientId));
        }

        [HttpPost]
        [Authorize(Policy = "CustomerDataPolicy")]
        public async Task<ActionResult<BookingRequest>>CreateBookingRequest([FromBody] BookingRequest_Create bookingRequest, [FromRoute] Guid clientId)
        {
            return new OkObjectResult(await _bookingRequestService.CreateBooking(bookingRequest, clientId));
        }

        [HttpPut]
        [Authorize(Policy = "CustomerDataPolicy")]
        public async Task<ActionResult<BookingRequest>> UpdateBookingRequest([FromBody] BookingRequest bookingRequest, [FromRoute] Guid clientId)
        {
            return new OkObjectResult(await _bookingRequestService.UpdateBooking(bookingRequest, clientId));
        }

        [HttpDelete("{bookingId}")]
        [Authorize(Policy = "CustomerDataPolicy")]
        public async Task<ActionResult<BookingRequest>> DeleteBookingRequest([FromRoute] Guid bookingId, [FromRoute] Guid clientId)
        {
            await _bookingRequestService.DeleteBooking(bookingId, clientId);
            return new OkResult();
        }

        [HttpPost("{bookingId}/confirm")]
        [Authorize(Policy = "CustomerDataPolicy")]
        public async Task<ActionResult<BookingRequest>> ConfirmBooking([FromRoute] Guid bookingId, [FromRoute] Guid clientId)
        {
            await _bookingRequestService.ConfirmBooking(bookingId, clientId);
            return new OkResult();
        }

        [HttpPost("{bookingId}/cancel")]
        [Authorize(Policy = "CustomerDataPolicy")]
        public async Task<ActionResult<BookingRequest>> CancelBooking([FromRoute] Guid bookingId, [FromRoute] Guid clientId)
        {
            await _bookingRequestService.CancelBooking(bookingId, clientId);
            return new OkResult();
        }
    }
}