using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gibson.Shared.Repositories.Tests;
using Microsoft.Extensions.Options;
using Moq;
using TechDevs.Customers;
using TechDevs.Shared.Models;
using Xunit;

namespace Gibson.BookingRequests
{
    public class BookingRequestServiceTests : IClassFixture<DatabaseTestFixture>
    {
        private readonly IBookingRequestRepository repo;
        private readonly ICustomerService customerService;

        public BookingRequestServiceTests(DatabaseTestFixture fixture)
        {
            var dbSettings = new MongoDbSettings { ConnectionString = fixture.Db.ConnectionString, Database = "Testing" };
            repo = new BookingRequestsRepository(Options.Create(dbSettings));
            var mockCustService = new Mock<ICustomerService>();
            var cust = new Customer
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = new DBRef { Id = Guid.NewGuid().ToString() },
                FirstName = "FirstName",
                LastName = "LastName",
                CustomerData = new CustomerData
                {
                    MyVehicles = new List<CustomerVehicle>
                    {
                        new CustomerVehicle { Registration = "EF02VCC" }
                    }
                }
            };
            mockCustService.Setup(x => x.GetById(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(cust));
            customerService = mockCustService.Object;
        }

        [Fact]
        public async Task GetBookings_Should_ReturnListOfBookingRequests()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId1 = Guid.NewGuid();
            var customerId2 = Guid.NewGuid();
            await repo.Create(new BookingRequest(), customerId1, clientId);
            await repo.Create(new BookingRequest(), customerId2, clientId);
            var sut = new BookingRequestService(repo, null);
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
            await repo.Create(new BookingRequest(), customerId1, clientId);
            await repo.Create(new BookingRequest(), customerId2, clientId);
            var sut = new BookingRequestService(repo, null);
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
            await repo.Create(new BookingRequest(), customerId1, clientId);
            await repo.Create(new BookingRequest(), customerId2, clientId);
            var sut = new BookingRequestService(repo, null);
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
            var entity = await repo.Create(new BookingRequest { Confirmed = true }, customerId, clientId);
            var sut = new BookingRequestService(repo, null);
            // Act
            await sut.CancelBooking(entity.Id, customerId, clientId);
            // Assert 
            var result = await repo.FindById(entity.Id, customerId, clientId);
            Assert.True(result.Cancelled);
        }

        [Fact]
        public async Task CancelBooking_Should_SetConfirmedToFalse()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await repo.Create(new BookingRequest { Confirmed = true }, customerId, clientId);
            var sut = new BookingRequestService(repo, null);
            // Act
            await sut.CancelBooking(entity.Id, customerId, clientId);
            // Assert 
            var result = await repo.FindById(entity.Id, customerId, clientId);
            Assert.False(result.Confirmed);
        }

        [Fact]
        public async Task ConfirmBooking_Should_SetConfirmedToTrue()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await repo.Create(new BookingRequest(), customerId, clientId);
            var sut = new BookingRequestService(repo, null);
            // Act
            await sut.ConfirmBooking(entity.Id, customerId, clientId);
            // Assert 
            var result = await repo.FindById(entity.Id, customerId, clientId);
            Assert.True(result.Confirmed);
        }

        [Fact]
        public async Task ConfirmBooking_Should_SetCancelledToFalse()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await repo.Create(new BookingRequest(), customerId, clientId);
            var sut = new BookingRequestService(repo, null);
            // Act
            await sut.ConfirmBooking(entity.Id, customerId, clientId);
            // Assert 
            var result = await repo.FindById(entity.Id, customerId, clientId);
            Assert.False(result.Cancelled);
        }

        [Fact]
        public async Task CreateBooking_Should_IncreaseRowCountByOne()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var beforeCount = (await repo.FindAll(customerId, clientId)).Count();
            var sut = new BookingRequestService(repo, customerService);
            var newBooking = new BookingRequest_Create { Registration = "EF02VCC" };
            // Act
            await sut.CreateBooking(newBooking, customerId, clientId);
            // Assert 
            var afterCount = (await repo.FindAll(customerId, clientId)).Count();
            Assert.Equal(beforeCount + 1, afterCount);
        }

        [Fact]
        public async Task DeleteBooking_Should_DecreaseRowCountByOne()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var sut = new BookingRequestService(repo, customerService);
            var newBooking = new BookingRequest_Create { Registration = "EF02VCC" };
            var created = await sut.CreateBooking(newBooking, customerId, clientId);
            var beforeCount = (await repo.FindAll(customerId, clientId)).Count();
            // Act
            await sut.DeleteBooking(created.Id, customerId, clientId);
            // Assert 
            var afterCount = (await repo.FindAll(customerId, clientId)).Count();
            Assert.Equal(beforeCount - 1, afterCount);
        }

    }
}
