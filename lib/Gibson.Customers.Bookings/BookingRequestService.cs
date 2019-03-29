using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Gibson.Customers.Vehicles;
using Gibson.Users;
using Gibson.Common.Models;

namespace Gibson.Customers.Bookings
{

    public class BookingRequestService : IBookingRequestService
    {
        private readonly IBookingRequestRepository bookings;
        private readonly IUserService userService;
        private readonly ICustomerVehicleService _vehicleService;

        public BookingRequestService(IBookingRequestRepository bookings, IUserService userService, ICustomerVehicleService vehicleService)
        {
            this.bookings = bookings;
            this.userService = userService;
            _vehicleService = vehicleService;
        }

        public async Task<List<BookingRequest>> GetBookings(Guid clientId)
        {
            return await bookings.FindAll(clientId);
        }

        public async Task<List<BookingRequest>> GetBookingsByCustomer(Guid customerId, Guid clientId)
        {
            return await bookings.FindAllByCustomer(customerId, clientId);
        }

        public async Task<BookingRequest> GetBooking(Guid id, Guid clientId)
        {
            return await bookings.FindById(id, clientId);
        }

        public async Task CancelBooking(Guid id, Guid clientId)
        {
            var booking = await bookings.FindById(id, clientId);
            booking.Cancelled = true;
            booking.Confirmed = false;
            var result = await bookings.Update(booking, clientId);
        }

        public async Task ConfirmBooking(Guid id, Guid clientId)
        {
            var booking = await bookings.FindById(id, clientId);
            booking.Confirmed = true;
            booking.ConfirmationEmailSent = true;
            var result = await bookings.Update(booking, clientId);
        }

        public async Task<BookingRequest> CreateBooking(BookingRequest_Create req, Guid clientId)
        {
            if (req.CustomerId == Guid.Empty) throw new Exception("CustomerId was not set");
            var customer = await userService.FindById(req.CustomerId, clientId);
            if(customer == null) throw new Exception("Customer could not be found");
            var vehicle = await _vehicleService.GetCustomerVehicle(req.Registration, req.CustomerId, clientId);
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
                    Customer = new PublicUser(customer),
                    RequestDate = DateTime.UtcNow,
                };
                return await bookings.Create(booking, req.CustomerId, clientId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task DeleteBooking(Guid id, Guid clientId)
        {
            await bookings.Delete(id, clientId);
        }

        public async Task<BookingRequest> UpdateBooking(BookingRequest request, Guid clientId)
        {
            return await bookings.Update(request, clientId);
        }

    }
}
