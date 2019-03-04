using System.Threading.Tasks;
using TechDevs.Shared.Models;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System;
using Gibson.Shared.Repositories.Tests;
using Microsoft.Extensions.Options;

namespace Gibson.CustomerVehicles
{
    public class CustomerVehicleServiceTests : IClassFixture<DatabaseTestFixture>
    {
        private readonly ICustomerVehicleRepository repo;

        public CustomerVehicleServiceTests(DatabaseTestFixture fixture)
        {
            var dbSettings = new MongoDbSettings { ConnectionString = fixture.Db.ConnectionString, Database = "Testing" };
            repo = new CustomerVehicleRespository(Options.Create(dbSettings));
        }

        // TODO: LastUpdated field not updated on update MotData
        // TODO: No test coverage for actually updating the vehicle record for MotData update

        [Fact]
        public async Task AddVehicleToCustomer_Should_ReturnAVehicle()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act 
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.IsType<CustomerVehicle>(result);
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_HaveOneVehicle()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var addResult = await sut.AddVehicleToCustomer("EF02VCC", customerId, clientId);
            // Assert
            var result = await repo.FindAll(clientId);
            Assert.True(result.Count == 1);
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_ThrowException_OnDuplicateRegistration()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            var req = "LD66OFZ";
            var custId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            await sut.AddVehicleToCustomer(req, custId, clientId);
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.AddVehicleToCustomer(req, custId, clientId));
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_HaveBasicVehicleInfo()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.Equal("RENAULT", result.Make.ToUpper());
            Assert.Equal("CLIO", result.Model.ToUpper());
            Assert.Equal("WHITE", result.Colour.ToUpper());
            Assert.Equal("PETROL", result.FuelType.ToUpper());
            Assert.Equal("EF02VCC", result.Registration.ToUpper());
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_HaveLastUpdated()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.IsType<DateTime>(result.LastUpdated);
            Assert.True(result.LastUpdated > DateTime.MinValue);
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_HaveInitializedMotData()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.IsType<MotData>(result.MotData);
            Assert.NotNull(result.MotData);
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_HaveInitializedMotResults()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.IsType<List<MotResult>>(result.MotData.MOTResults);
            Assert.NotNull(result.MotData.MOTResults);
        }


        [Fact]
        public async Task AddVehileToCustomer_Should_HaveAtleastOneMotHistory()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.True(result.MotData.MOTResults.Count > 0);
        }

        [Fact]
        public async Task AddVehileToCustomer_Should_HaveAtleastOneMotComment()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.True(result.MotData.MOTResults.FirstOrDefault().Comments.Count > 0);
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_HaveEmptyServiceData()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.NotNull(result.ServiceData);
        }

        [Fact]
        public async Task DeleteCustomerVehicle_Should_ThrowError_WhenVehicleDoesNotExists()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.DeleteCustomerVehicle(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteCustomerVehicle_Should_ReturnWithoutError()
        {
            // Arrange
            var v1 = new CustomerVehicle { CustomerId = Guid.NewGuid(), ClientId = Guid.NewGuid() };
            await repo.Create(v1, v1.CustomerId, v1.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var ex = await Record.ExceptionAsync(async () => await sut.DeleteCustomerVehicle(v1.Id, v1.CustomerId, v1.ClientId));
            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public async Task DeleteCustomerVehicle_Should_NotFindVehicleAfterDelete()
        {
            // Arrange
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            await sut.DeleteCustomerVehicle(v1.Id, v1.CustomerId, v1.ClientId);
            var result = (await repo.FindAllByCustomer(v1.CustomerId, v1.ClientId)).FirstOrDefault(x => x.Registration == "EF02VCC");
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCustomerVehicle_Should_ReturnSinlgeVehicle()
        {
            // Arrange
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.GetCustomerVehicle("EF02VCC", v1.CustomerId, v1.ClientId);
            // Assert
            Assert.IsType<CustomerVehicle>(result);
        }

        [Fact]
        public async Task GetCustomerVehicle_Should_ReturnNull_WhenDoesNotExist()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.GetCustomerVehicle("SHOULDNOTEXIST", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task GetCustomerVehicleById_Should_ReturnSinlgeVehicle()
        {
            // Arrange
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.GetCustomerVehicle(v1.Id, v1.ClientId);
            // Assert
            Assert.IsType<CustomerVehicle>(result);
        }

        [Fact]
        public async Task GetCustomerVehicleById_Should_ReturnNull_WhenDoesNotExist()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.GetCustomerVehicle(Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task UpdateCustomerVehicle_Should_ReturnVehicle()
        {
            // Arrange
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateCustomerVehicle(v1, v1.ClientId);
            // Assert
            Assert.IsType<CustomerVehicle>(result);
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_ReturnVehicle_WithUpdatedData()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, customerId, clientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            var newValue = DateTime.UtcNow;
            v1.Colour = "SomeNewColour";
            // Act
            var result = await sut.UpdateCustomerVehicle(v1, clientId);
            // Assert
            Assert.Equal("SomeNewColour", result.Colour);
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_ThrowException_WhenDoesNotExist()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            var updatedVehicle = new CustomerVehicle { Registration = "SHOULDNOTEXIST", LastUpdated = DateTime.UtcNow };
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.UpdateCustomerVehicle(updatedVehicle, Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_ReturnVehicle_WithUpdatedDataFromGet()
        {
            // Arrange
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            v1.Colour = "SomeNewColour";
            // Act
            await sut.UpdateCustomerVehicle(v1, v1.ClientId);
            var result = (await repo.FindAllByCustomer(v1.CustomerId, v1.ClientId)).FirstOrDefault(x => x.Registration == v1.Registration);
            // Assert
            Assert.Equal("SomeNewColour", v1.Colour);
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_HaveMatchingRegistration()
        {
            // Arrange
            var v1 = new CustomerVehicle
            {
                Registration = "EF02VCC",
                LastUpdated = DateTime.UtcNow,
                ClientId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid()
            };
            await repo.Create(v1, v1.CustomerId, v1.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            v1.LastUpdated = DateTime.UtcNow;
            // Act
            await sut.UpdateCustomerVehicle(v1, v1.ClientId);
            var result = (await repo.FindAllByCustomer(v1.CustomerId, v1.ClientId)).FirstOrDefault(x => x.Registration == v1.Registration);
            // Assert
            Assert.Equal(result.Registration, v1.Registration);
        }

        [Fact]
        public async Task UpdateVehicleMotData_Should_ReturnCustomerVehicle()
        {
            // Arrange
            var dummyVehicle = new CustomerVehicle
            {
                CustomerId = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Registration = "LD66OFZ",
                MotData = new MotData { MOTExpiryDate = DateTime.Parse("2019-01-01") }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.ClientId);
            // Assert
            Assert.IsType<CustomerVehicle>(result);
        }

        [Fact]
        public async Task UpdateVehicleMotData_Should_ReturnModifiedMotData()
        {
            // Arrange
            var dummyVehicle = new CustomerVehicle
            {
                CustomerId = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Registration = "LD66OFZ",
                MotData = new MotData { MOTExpiryDate = DateTime.Parse("2019-01-01") }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.ClientId);
            // Assert
            Assert.NotNull(result.MotData);
        }

        [Fact]
        public async Task UpdateVehicleMotData_Should_ReturnUpdatedAfterFetch()
        {
            // Arrange
            var currentExpiryDate = DateTime.Parse("2019-01-01");
            var dummyVehicle = new CustomerVehicle
            {
                CustomerId = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Registration = "LD66OFZ",
                MotData = new MotData { MOTExpiryDate = currentExpiryDate }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.ClientId);
            // Act
            var result = await repo.FindById(dummyVehicle.Id, dummyVehicle.ClientId);
            // Assert
            Assert.True(result.MotData.MOTExpiryDate > currentExpiryDate);
        }

        [Fact]
        public async Task UpdateVehicleData_Should_ReturnExpiryDateLaterThanBeforeUpdate()
        {
            // Arrange
            var currentExpiryDate = DateTime.Parse("2019-01-01");
            var dummyVehicle = new CustomerVehicle
            {
                CustomerId = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Registration = "LD66OFZ",
                MotData = new MotData { MOTExpiryDate = currentExpiryDate }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.ClientId);
            // Assert
            Assert.True(result.MotData.MOTExpiryDate > currentExpiryDate);
        }

        [Fact]
        public async Task UpdateVehicleData_Should_ReturnWithSameHeaderData()
        {
            // Arrange
            var dummyVehicle = new CustomerVehicle
            {
                CustomerId = Guid.NewGuid(),
                ClientId = Guid.NewGuid(),
                Registration = "LD66OFZ",
                Make = "KIA",
                Model = "CEED",
                Colour = "BLACK",
                MotData = new MotData { MOTExpiryDate = DateTime.Parse("2019-01-01") }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.ClientId);
            // Assert
            Assert.Equal(dummyVehicle.Model.ToUpper(), result.Model.ToUpper());
            Assert.Equal(dummyVehicle.Make.ToUpper(), result.Make.ToUpper());
            Assert.Equal(dummyVehicle.Registration.ToUpper(), result.Registration.ToUpper());
            Assert.Equal(dummyVehicle.Colour.ToUpper(), result.Colour.ToUpper());
        }

        [Fact]
        public async Task UpdateServiceData_Should_ReturnCustomerVehicle()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            var vehicle = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            var serviceData = new ServiceData
            {
                EstAnualMileage = 8000,
                MaxMileage = 6000,
                MaxMonths = 12,
                ServiceDataConfiguredBy = "Customer"
            };
            // Act
            var result = await sut.UpdateServiceData(serviceData, vehicle.Id, vehicle.ClientId);
            // Assert
            Assert.IsType<CustomerVehicle>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateServiceData_Should_ThrowException_WhenVehicleDoesNotExist()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.UpdateServiceData(new ServiceData(), Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateServiceData_Should_ReturnWithMatchingServiceData()
        {
            // Arrange
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            var vehicle = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            var serviceData = new ServiceData
            {
                EstAnualMileage = 8000,
                MaxMileage = 6000,
                MaxMonths = 12,
                ServiceDataConfiguredBy = "Customer"
            };
            // Act
            var result = await sut.UpdateServiceData(serviceData, vehicle.Id, vehicle.ClientId);
            // Assert
            Assert.Equal(serviceData.EstAnualMileage, result.ServiceData.EstAnualMileage);
            Assert.Equal(serviceData.MaxMonths, result.ServiceData.MaxMonths);
            Assert.Equal(serviceData.MaxMileage, result.ServiceData.MaxMileage);
            Assert.Equal(serviceData.ServiceDataConfiguredBy, result.ServiceData.ServiceDataConfiguredBy);
        }
    }
}