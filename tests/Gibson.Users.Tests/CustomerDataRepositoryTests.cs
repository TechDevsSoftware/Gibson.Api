using System;
using System.Threading.Tasks;
using Gibson.Common.Models;
using Gibson.Shared.Repositories;
using Gibson.Tests.Common;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gibson.Users.Tests
{
    public class CustomerDataRepositoryTests : IClassFixture<DatabaseTestFixture>
    {
        private IOptions<MongoDbSettings> _settings;

        public CustomerDataRepositoryTests(DatabaseTestFixture fixture)
        {
            var connString = fixture.Db.ConnectionString;
            var dbSettings = new MongoDbSettings { ConnectionString = connString, Database = "Testing" };
            _settings = Options.Create(dbSettings);
        }

        private ICustomerDataRepository<MockCustomerEntity> GetMockRepo() =>
            new CustomerDataRepository<MockCustomerEntity>("DummyCollection",_settings);

        [Fact]
        public async Task Create_Should_IncreaseCollectionCount_ByOne()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var beforeCollection = await repo.FindAllByCustomer(clientId, customerId);
            // Act
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            // Assert 
            var afterCollection = await repo.FindAllByCustomer(customerId, clientId);
            Assert.True(afterCollection.Count == beforeCollection.Count + 1);
        }

        [Fact]
        public async Task Create_Should_SetCustomerId()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            // Assert 
            Assert.NotEqual(Guid.Empty, result.CustomerId);
        }

        [Fact]
        public async Task Create_Should_SetClientId()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            // Assert 
            Assert.Equal(clientId, result.ClientId);
        }

        [Fact]
        public async Task Create_Should_SetId()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            // Assert 
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task Create_Should_ReturnCustomerEntity()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            // Assert 
            Assert.IsType<MockCustomerEntity>(result);
        }

        [Fact]
        public async Task Create_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Create(new MockCustomerEntity(), customerId, clientId));
        }

        [Fact]
        public async Task Delete_Should_DecreaseCollecitonCount_ByOne()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            var beforeCollection = await repo.FindAllByCustomer(customerId, clientId);
            // Act
            await repo.Delete(result.Id, clientId);
            // Assert 
            var afterCollection = await repo.FindAllByCustomer(customerId, clientId);
            Assert.True(afterCollection.Count == beforeCollection.Count - 1);
        }

        [Fact]
        public async Task Delete_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Delete(Guid.NewGuid(), clientId));
        }

        [Fact]
        public async Task Update_Should_PersistModifiedProperty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var obj = new MockCustomerEntity { TestField = "BeforeValue" };
            var entity = await repo.Create(obj, customerId, clientId);
            entity.TestField = "NewValue";
            // Act
            var result = await repo.Update(entity, clientId);
            var after = await repo.FindById(entity.Id, clientId);
            // Assert
            Assert.Equal("NewValue", after.TestField);
        }

        [Fact]
        public async Task Update_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Update(new MockCustomerEntity(), clientId));
        }

        [Fact]
        public async Task FindAll_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.FindAllByCustomer(customerId, clientId));
        }

        [Fact]
        public async Task FindById_Should_ReturnNull_When_ClientId_NotMatching_Result()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            var wrongClientId = Guid.NewGuid();
            // Act
            var result = await repo.FindById(entity.Id, wrongClientId);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindById_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.FindById(Guid.NewGuid(), clientId));
        }
    }
}
