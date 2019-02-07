using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TechDevs.Customers;
using TechDevs.Shared.Models;

namespace Gibson.BookingRequests
{

    public class BookingRequestService : IBookingRequestService
    {
        private readonly ICustomerDataRepository<BookingRequest> bookings;
        private readonly ICustomerService customers;

        public BookingRequestService(ICustomerDataRepository<BookingRequest> bookings, ICustomerService customers)
        {
            this.bookings = bookings;
            this.customers = customers;
        }

        public async Task<List<BookingRequest>> GetBookings(Guid clientId)
        {
            return await bookings.FindAllAnyCustomer(clientId);
        }

        public async Task<List<BookingRequest>> GetBookingsByCustomer(Guid customerId, Guid clientId)
        {
            return await bookings.FindAll(customerId, clientId);
        }

        public async Task<BookingRequest> GetBooking(Guid id, Guid customerId, Guid clientId)
        {
            return await bookings.FindById(id, customerId, clientId);
        }

        public async Task CancelBooking(Guid id, Guid customerId, Guid clientId)
        {
            var booking = await bookings.FindById(id, customerId, clientId);
            booking.Cancelled = true;
            booking.Confirmed = false;
            var result = await bookings.Update(booking, customerId, clientId);
        }

        public async Task ConfirmBooking(Guid id, Guid customerId, Guid clientId)
        {
            var booking = await bookings.FindById(id, customerId, clientId);
            booking.Confirmed = true;
            booking.ConfirmationEmailSent = true;
            var result = await bookings.Update(booking, customerId, clientId);
        }

        public async Task<BookingRequest> CreateBooking(BookingRequest_Create req, Guid customerId, Guid clientId)
        {
            var customer = await customers.GetById(customerId.ToString(), clientId.ToString());
            var vehicle = customer?.CustomerData?.MyVehicles?.FirstOrDefault(x => x.Registration == req.Registration);
            try
            {
                var booking = new BookingRequest
                {
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
                    Customer = new BookingCustomer
                    {
                        Id = Guid.Parse(customer.Id),
                        ClientId = Guid.Parse(customer.ClientId.Id),
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        EmailAddress = customer.EmailAddress,
                        ContactNumber = customer.ContactNumber
                    },
                    RequestDate = DateTime.UtcNow,
                };
                return await bookings.Create(booking, customerId, clientId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task DeleteBooking(Guid id, Guid customerId, Guid clientId)
        {
            await bookings.Delete(id, customerId, clientId);
        }

        public async Task<BookingRequest> UpdateBooking(BookingRequest request, Guid customerId, Guid clientId)
        {
            return await bookings.Update(request, customerId, clientId);
        }

    }
}
