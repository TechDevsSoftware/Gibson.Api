using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.BookingRequests
{
    public interface IBookingRequestsRepository
    {
        Task<BookingRequest> Create(BookingRequest entity, string clientIdOrKey);
        Task Delete(string id, string clientIdOrKey);
        Task<List<BookingRequest>> FindAll(string clientIdOrKey);
        Task<BookingRequest> FindById(string id, string clientIdOrKey);
        Task<BookingRequest> Update(BookingRequest entity, string clientIdOrKey);
    }
}