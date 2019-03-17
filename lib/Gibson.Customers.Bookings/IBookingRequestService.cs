using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Customers.Bookings
{
    public interface IBookingRequestService
    {
        Task<List<BookingRequest>> GetBookingsByCustomer(Guid customerId, Guid clientId);
        Task<List<BookingRequest>> GetBookings(Guid clientId);
        Task<BookingRequest> GetBooking(Guid id, Guid clientId);
        Task<BookingRequest> CreateBooking(BookingRequest_Create req, Guid clientId);
        Task<BookingRequest> UpdateBooking(BookingRequest request, Guid clientId);
        Task DeleteBooking(Guid id, Guid clientId);
        Task ConfirmBooking(Guid id, Guid clientId);
        Task CancelBooking(Guid id, Guid clientId);
    }
}
