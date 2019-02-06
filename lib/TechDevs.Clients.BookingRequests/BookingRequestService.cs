using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechDevs.Clients;
using TechDevs.Customers;
using TechDevs.Shared.Models;

namespace Gibson.BookingRequests
{
    public class BookingRequestService : IBookingRequestService
    {
        private readonly IBookingRequestsRepository bookings;
        private readonly IClientService clients;
        private readonly ICustomerService customers;

        public BookingRequestService(IBookingRequestsRepository bookings, IClientService clients, ICustomerService customers)
        {
            this.bookings = bookings;
            this.clients = clients;
            this.customers = customers;
        }

        public async Task<List<BookingRequest>> GetBookings(string clientId)
        {
            return await bookings.FindAll(clientId);
        }

        public async Task<BookingRequest> GetBooking(string id, string clientId)
        {
            return await bookings.FindById(id, clientId);
        }

        public async Task CancelBooking(string id, string clientId)
        {
            var client = await clients.GetClient(clientId);
            var booking = await bookings.FindById(id, client.Id);
            booking.Cancelled = true;
            booking.Confirmed = false;
            var result = await bookings.Update(booking, client.Id);
        }

        public async Task ConfirmBooking(string id, string clientId)
        {
            var client = await clients.GetClient(clientId);
            var booking = await bookings.FindById(id, client.Id);
            booking.Confirmed = true;
            booking.ConfirmationEmailSent = true;
            var result = await bookings.Update(booking,client.Id);
        }

        public async Task<BookingRequest> CreateBooking(BookingRequest_Create req, string userId, string clientId)
        {
            var client = await clients.GetClient(clientId);
            var customer = await customers.GetById(userId, clientId);
            var vehicle = customer?.CustomerData?.MyVehicles?.FirstOrDefault(x => x.Registration == req.Registration);

            var booking = new BookingRequest
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = clientId,
                CustomerId = customer.Id,
                Registration = req.Registration,
                MOT = req.MotRequest,
                Service = req.ServiceRequest,
                PreferedDate = req.PreferedDate,
                PreferedTime = req.PreferedTime,
                Message = req.Message,
                Confirmed = false,
                Cancelled = false,
                ConfirmationEmailSent = false,
                Vehicle = vehicle,
                Customer = new BookingCustomer(customer),
                RequestDate = DateTime.UtcNow,
            };

            var result = await bookings.Create(booking, client.Id);
            return result;
        }

        public async Task DeleteBooking(string id, string clientId)
        {
            var client = await clients.GetClient(clientId);
            await bookings.Delete(id, clientId);
        }

        public async Task<BookingRequest> UpdateBooking(BookingRequest request, string clientId)
        {
            var client = await clients.GetClient(clientId);
            return await bookings.Update(request, clientId);
        }

    }
}
