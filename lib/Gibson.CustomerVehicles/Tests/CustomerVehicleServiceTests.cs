using System.Threading.Tasks;
using TechDevs.Shared.Models;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System;

namespace Gibson.CustomerVehicles
{
    public class CustomerVehicleServiceTests
    {
        // TODO: LastUpdated field not updated on update MotData
        // TODO: No test coverage for actually updating the cehicle record for MotData update

        [Fact]
        public async Task AddVehicleToCustomer_Should_ReturnAVehicle()
        {
            // Arrange
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
            // Act 
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.IsType<CustomerVehicle>(result);
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_HaveOneVehicle()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            repo.Reset();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var sut = new CustomerVehicleService(repo, new VehicleDataService());
            // Act
            var addResult = await sut.AddVehicleToCustomer("EF02VCC", customerId, clientId);
            // Assert
            Assert.True(repo.RowCount() == 1);
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_ThrowException_OnDuplicateRegistration()
        {
            // Arrange
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.AddVehicleToCustomer("LD66OFZ", Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task AddVehicleToCustomer_Should_HaveBasicVehicleInfo()
        {
            // Arrange
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
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
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
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
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
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
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
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
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.True(result.MotData.MOTResults.Count > 0);
        }

        [Fact]
        public async Task AddVehileToCustomer_Should_HaveAtleastOneMotComment()
        {
            // Arrange
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
            // Act
            var result = await sut.AddVehicleToCustomer("EF02VCC", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.True(result.MotData.MOTResults.FirstOrDefault().Comments.Count > 0);
        }

        [Fact]
        public async Task DeleteCustomerVehicle_Should_ThrowError_WhenVehicleDoesNotExists()
        {
            // Arrange
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.DeleteCustomerVehicle(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteCustomerVehicle_Should_ReturnWithoutError()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var v1 = new CustomerVehicle();
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
            var repo = new MockCustomerVehicleRepo();
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            await sut.DeleteCustomerVehicle(v1.Id, v1.CustomerId, v1.ClientId);
            var result = (await repo.FindAll(v1.CustomerId, v1.ClientId)).FirstOrDefault(x => x.Registration == "EF02VCC");
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCustomerVehicle_Should_ReturnAsSinlgeVehicle()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
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
            var sut = new CustomerVehicleService(new MockCustomerVehicleRepo(), new MockVehicleDataService());
            // Act
            var result = await sut.GetCustomerVehicle("SHOULDNOTEXIST", Guid.NewGuid(), Guid.NewGuid());
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_ReturnVehicle()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateCustomerVehicle(v1, v1.CustomerId, v1.ClientId);
            // Assert
            Assert.IsType<CustomerVehicle>(result);
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_ReturnVehicle_WithUpdatedData()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            v1.LastUpdated = DateTime.UtcNow;
            // Act
            var result = await sut.UpdateCustomerVehicle(v1, v1.CustomerId, v1.ClientId);
            // Assert
            Assert.Equal(result.LastUpdated, v1.LastUpdated);
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_ThrowException_WhenDoesNotExist()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            var updatedVehicle = new CustomerVehicle { Registration = "SHOULDNOTEXIST", LastUpdated = DateTime.UtcNow };
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.UpdateCustomerVehicle(updatedVehicle, Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_ReturnVehicle_WithUpdatedDataFromGet()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var v1 = new CustomerVehicle { Registration = "EF02VCC" };
            await repo.Create(v1, Guid.NewGuid(), Guid.NewGuid());
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            v1.LastUpdated = DateTime.UtcNow;
            // Act
            await sut.UpdateCustomerVehicle(v1, v1.CustomerId, v1.ClientId);
            var result = (await repo.FindAll(v1.CustomerId, v1.ClientId)).FirstOrDefault(x => x.Registration == v1.Registration);
            // Assert
            Assert.Equal(result.LastUpdated, v1.LastUpdated);
        }

        [Fact]
        public async Task UpdateCustomerVehicle_Should_HaveMatchingRegistration()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
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
            await sut.UpdateCustomerVehicle(v1, v1.CustomerId, v1.ClientId);
            var result = (await repo.FindAll(v1.CustomerId, v1.ClientId)).FirstOrDefault(x => x.Registration == v1.Registration);
            // Assert
            Assert.Equal(result.Registration, v1.Registration);
        }

        [Fact]
        public async Task UpdateVehicleMotData_Should_ReturnCustomerVehicle()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var dummyVehicle = new CustomerVehicle
            {
                Registration = "LD66OFZ",
                MotData = new MotData { MOTExpiryDate = DateTime.Parse("2019-01-01") }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            // Assert
            Assert.IsType<CustomerVehicle>(result);
        }

        [Fact]
        public async Task UpdateVehicleMotData_Should_ReturnModifiedMotData()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var dummyVehicle = new CustomerVehicle
            {
                Registration = "LD66OFZ",
                MotData = new MotData { MOTExpiryDate = DateTime.Parse("2019-01-01") }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            // Assert
            Assert.NotNull(result.MotData);
        }

        [Fact]
        public async Task UpdateVehicleMotData_Should_ReturnUpdatedAfterFetch()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var currentExpiryDate = DateTime.Parse("2019-01-01");
            var dummyVehicle = new CustomerVehicle
            {
                Registration = "LD66OFZ",
                MotData = new MotData { MOTExpiryDate = currentExpiryDate }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            // Act
            var result = await repo.FindById(dummyVehicle.Id, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            // Assert
            Assert.True(result.MotData.MOTExpiryDate > currentExpiryDate);
        }

        [Fact]
        public async Task UpdateVehicleData_Should_ReturnExpiryDateLaterThanBeforeUpdate()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var currentExpiryDate = DateTime.Parse("2019-01-01");
            var dummyVehicle = new CustomerVehicle
            {
                Registration = "LD66OFZ",
                MotData = new MotData { MOTExpiryDate = currentExpiryDate }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            // Assert
            Assert.True(result.MotData.MOTExpiryDate > currentExpiryDate);
        }

        [Fact]
        public async Task UpdateVehicleData_Should_ReturnWithSameHeaderData()
        {
            // Arrange
            var repo = new MockCustomerVehicleRepo();
            var dummyVehicle = new CustomerVehicle
            {
                Registration = "LD66OFZ",
                Make = "KIA",
                Model = "CEED",
                Colour = "BLACK",
                MotData = new MotData { MOTExpiryDate = DateTime.Parse("2019-01-01") }
            };
            await repo.Create(dummyVehicle, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            var sut = new CustomerVehicleService(repo, new MockVehicleDataService());
            // Act
            var result = await sut.UpdateMotData(dummyVehicle.Id, dummyVehicle.CustomerId, dummyVehicle.ClientId);
            // Assert
            Assert.Equal(dummyVehicle.Model.ToUpper(), result.Model.ToUpper());
            Assert.Equal(dummyVehicle.Make.ToUpper(), result.Make.ToUpper());
            Assert.Equal(dummyVehicle.Registration.ToUpper(), result.Registration.ToUpper());
            Assert.Equal(dummyVehicle.Colour.ToUpper(), result.Colour.ToUpper());
        }
    }
}