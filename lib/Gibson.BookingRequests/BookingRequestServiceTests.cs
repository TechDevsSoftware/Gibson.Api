using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gibson.CustomerVehicles;
using Gibson.Shared.Repositories.Tests;
using Gibson.Users;
using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using Xunit;

namespace Gibson.BookingRequests
{
    public class BookingRequestServiceTests : IClassFixture<DatabaseTestFixture>
    {
        private readonly IUserRepository userRepo;
        private readonly ICustomerVehicleRepository vehicleRepo;
        private readonly IBookingRequestRepository bookingRepo;
        
        private readonly IUserService userService;
        private readonly ICustomerVehicleService vehicleService;
        private readonly IVehicleDataService vehicleDataService;
        private readonly IBookingRequestService sut;
        
        public BookingRequestServiceTests(DatabaseTestFixture fixture)
        {
            var dbSettings = new MongoDbSettings { ConnectionString = fixture.Db.ConnectionString, Database = "Testing" };
            bookingRepo = new BookingRequestsRepository(Options.Create(dbSettings));
            userRepo = new UserRepository("Users", Options.Create(dbSettings));
            vehicleRepo = new CustomerVehicleRespository(Options.Create(dbSettings));
            userService = new UserService(userRepo);
            vehicleDataService = new VehicleDataService();
            vehicleService = new CustomerVehicleService(vehicleRepo, vehicleDataService);
            sut = new BookingRequestService(bookingRepo, userService, vehicleService);
        }

        [Fact]
        public async Task GetBookings_Should_ReturnListOfBookingRequests()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId1 = Guid.NewGuid();
            var customerId2 = Guid.NewGuid();
            await bookingRepo.Create(new BookingRequest(), customerId1, clientId);
            await bookingRepo.Create(new BookingRequest(), customerId2, clientId);
            // Act
            var result = await sut.GetBookings(clientId);
            // Assert 
            Assert.IsType<List<BookingRequest>>(result);
        }

        [Fact]
        public async Task GetBookings_Should_ReturnForMultipleCustomers()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId1 = Guid.NewGuid();
            var customerId2 = Guid.NewGuid();
            await bookingRepo.Create(new BookingRequest(), customerId1, clientId);
            await bookingRepo.Create(new BookingRequest(), customerId2, clientId);
            // Act
            var result = await sut.GetBookings(clientId);
            // Assert 
            Assert.True(result.Select(x => x.CustomerId).Distinct().Count() == 2);
        }

        [Fact]
        public async Task GetBookingsByCustomer_Should_ReturnSingleCustomer()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId1 = Guid.NewGuid();
            var customerId2 = Guid.NewGuid();
            await bookingRepo.Create(new BookingRequest(), customerId1, clientId);
            await bookingRepo.Create(new BookingRequest(), customerId2, clientId);
            // Act
            var result = await sut.GetBookingsByCustomer(customerId1, clientId);
            // Assert 
            Assert.True(result.Select(x => x.CustomerId).Distinct().Count() == 1);
        }

        [Fact]
        public async Task CancelBooking_Should_SetCancelledToTrue()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await bookingRepo.Create(new BookingRequest { Confirmed = true }, customerId, clientId);
            // Act
            await sut.CancelBooking(entity.Id, clientId);
            // Assert 
            var result = await bookingRepo.FindById(entity.Id, clientId);
            Assert.True(result.Cancelled);
        }

        [Fact]
        public async Task CancelBooking_Should_SetConfirmedToFalse()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await bookingRepo.Create(new BookingRequest { Confirmed = true }, customerId, clientId);
            // Act
            await sut.CancelBooking(entity.Id, clientId);
            // Assert 
            var result = await bookingRepo.FindById(entity.Id, clientId);
            Assert.False(result.Confirmed);
        }

        [Fact]
        public async Task ConfirmBooking_Should_SetConfirmedToTrue()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await bookingRepo.Create(new BookingRequest(), customerId, clientId);
            // Act
            await sut.ConfirmBooking(entity.Id, clientId);
            // Assert 
            var result = await bookingRepo.FindById(entity.Id, clientId);
            Assert.True(result.Confirmed);
        }

        [Fact]
        public async Task ConfirmBooking_Should_SetCancelledToFalse()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await bookingRepo.Create(new BookingRequest(), customerId, clientId);
            // Act
            await sut.ConfirmBooking(entity.Id, clientId);
            // Assert 
            var result = await bookingRepo.FindById(entity.Id, clientId);
            Assert.False(result.Cancelled);
        }

        [Fact]
        public async Task CreateBooking_Should_IncreaseRowCountByOne()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var beforeCount = (await bookingRepo.FindAllByCustomer(customerId, clientId)).Count();
            var newBooking = new BookingRequest_Create { Registration = "EF02VCC", CustomerId = customerId };
            // Act
            await sut.CreateBooking(newBooking, clientId);
            // Assert 
            var afterCount = (await bookingRepo.FindAllByCustomer(customerId, clientId)).Count();
            Assert.Equal(beforeCount + 1, afterCount);
        }

        [Fact]
        public async Task DeleteBooking_Should_DecreaseRowCountByOne()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var newBooking = new BookingRequest_Create { Registration = "EF02VCC", CustomerId = customerId };
            var created = await sut.CreateBooking(newBooking, clientId);
            var beforeCount = (await bookingRepo.FindAllByCustomer(customerId, clientId)).Count();
            // Act
            await sut.DeleteBooking(created.Id, clientId);
            // Assert 
            var afterCount = (await bookingRepo.FindAllByCustomer(customerId, clientId)).Count();
            Assert.Equal(beforeCount - 1, afterCount);
        }

        [Fact]
        public async Task CreateBooking_Should_ThrowException_WhenCustomerIdIsNull()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var newBooking = new BookingRequest_Create { Registration = "EF02VCC" };
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await sut.CreateBooking(newBooking, clientId));
        }

    }
}
