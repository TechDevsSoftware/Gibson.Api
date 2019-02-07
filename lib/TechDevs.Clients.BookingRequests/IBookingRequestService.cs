using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.BookingRequests
{
    public interface IBookingRequestService
    {
        Task<List<BookingRequest>> GetBookingsByCustomer(Guid customerId, Guid clientId);
        Task<List<BookingRequest>> GetBookings(Guid clientId);
        Task<BookingRequest> GetBooking(Guid id, Guid customerId, Guid clientId);
        Task<BookingRequest> CreateBooking(BookingRequest_Create req, Guid customerId, Guid clientId);
        Task<BookingRequest> UpdateBooking(BookingRequest request, Guid customerId, Guid clientId);
        Task DeleteBooking(Guid id, Guid customerId, Guid clientId);
        Task ConfirmBooking(Guid id, Guid customerId, Guid clientId);
        Task CancelBooking(Guid id, Guid customerId, Guid clientId);
    }
}
