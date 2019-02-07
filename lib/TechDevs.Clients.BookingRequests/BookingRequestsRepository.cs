using Gibson.Shared.Repositories;
using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;

namespace Gibson.BookingRequests
{
    public class BookingRequestsRepository : CustomerDataRepository<BookingRequest>, IBookingRequestRepository
    {
        public BookingRequestsRepository(IOptions<MongoDbSettings> dbSettings)
        : base("BookingRequests", dbSettings)
        {
        }
    }
}
