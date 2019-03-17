using Gibson.Shared.Repositories;
using Microsoft.Extensions.Options;
using Gibson.Common.Models;

namespace Gibson.Customers.Bookings
{
    public class BookingRequestsRepository : CustomerDataRepository<BookingRequest>, IBookingRequestRepository
    {
        public BookingRequestsRepository(IOptions<MongoDbSettings> dbSettings)
        : base("BookingRequests", dbSettings)
        {
        }
    }
}
