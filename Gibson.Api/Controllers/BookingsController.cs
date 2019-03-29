using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;
using Gibson.Customers.Bookings;

namespace Gibson.Api.Controllers
{   
    [Route("clients/{clientId}/bookings")]
    public class BookingsController : Controller
    {
        private readonly IBookingRequestService _bookingRequestService;

        public BookingsController(IBookingRequestService bookingRequestService)
        {
            _bookingRequestService = bookingRequestService;
        }
        
        [HttpGet]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<List<BookingRequest>>> GetClientBookings([FromRoute] Guid clientId)
        {
            return new OkObjectResult(await _bookingRequestService.GetBookings(clientId));
        }
        
        [HttpGet("{customerId}")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<List<BookingRequest>>> GetCustomerBookings(
            [FromRoute] Guid customerId, 
            [FromRoute] Guid clientId)
        {
            return new OkObjectResult(await _bookingRequestService.GetBookingsByCustomer(customerId, clientId));
        }

        [HttpGet("{bookingId}")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<BookingRequest>> GetBooking(
            [FromRoute] Guid bookingId, 
            [FromRoute] Guid clientId)
        {
            return new OkObjectResult(await _bookingRequestService.GetBooking(bookingId, clientId));
        }

        [HttpPost("{customerId}")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<BookingRequest>>CreateBooking(
            [FromBody] BookingRequest_Create bookingRequest, 
            [FromRoute] Guid clientId,
            [FromRoute] Guid customerId
            )
        {
            bookingRequest.CustomerId = customerId;
            return new OkObjectResult(await _bookingRequestService.CreateBooking(bookingRequest, clientId));
        }

        [HttpPut("{bookingId}")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<BookingRequest>> UpdateBooking(
            [FromBody] BookingRequest bookingRequest,
            [FromRoute] Guid bookingId, 
            [FromRoute] Guid clientId
            )
        {
            return new OkObjectResult(await _bookingRequestService.UpdateBooking(bookingRequest, clientId));
        }

        [HttpDelete("{bookingId}")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<BookingRequest>> DeleteBooking(
            [FromRoute] Guid bookingId, 
            [FromRoute] Guid clientId
            )
        {
            await _bookingRequestService.DeleteBooking(bookingId, clientId);
            return new OkResult();
        }

        [HttpPost("{bookingId}/confirm")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<BookingRequest>> ConfirmBooking(
            [FromRoute] Guid bookingId, 
            [FromRoute] Guid clientId
            )
        {
            await _bookingRequestService.ConfirmBooking(bookingId, clientId);
            return new OkResult();
        }

        [HttpPost("{bookingId}/cancel")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<BookingRequest>> CancelBooking(
            [FromRoute] Guid bookingId, 
            [FromRoute] Guid clientId
            )
        {
            await _bookingRequestService.CancelBooking(bookingId, clientId);
            return new OkResult();
        }
    }
}