using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.BookingRequests
{
    public interface IBookingRequestService
    {
        Task<List<BookingRequest>> GetBookings(string clientId);
        Task<BookingRequest> GetBooking(string id, string clientId);
        Task<BookingRequest> CreateBooking(BookingRequest_Create req, string userId, string clientId);
        Task<BookingRequest> UpdateBooking(BookingRequest request, string clientId);
        Task DeleteBooking(string id, string clientId);
        Task ConfirmBooking(string id, string clientId);
        Task CancelBooking(string id, string clientId);
    }
}
