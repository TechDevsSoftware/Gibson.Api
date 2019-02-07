using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using Xunit;

namespace Gibson.Shared.Repositories.Tests
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

        [Fact]
        public async Task Create_Should_IncreaseCollecitonCount_ByOne()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var beforeCollection = await repo.FindAll(clientId, customerId);
            // Act
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            // Assert 
            var afterCollection = await repo.FindAll(customerId, clientId);
            Assert.True(afterCollection.Count == beforeCollection.Count + 1);
        }

        [Fact]
        public async Task Create_Should_SetCustomerId()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
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
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            // Assert 
            Assert.NotEqual(Guid.Empty, result.ClientId);
        }

        [Fact]
        public async Task Create_Should_SetId()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            // Assert 
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task Create_Should_ReuturnCustomerEntity()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
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
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Create(new MockCustomerEntity(), customerId, clientId));
        }

        [Fact]
        public async Task Create_Should_ThrowException_WhenCustomerId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.Empty;
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Create(new MockCustomerEntity(), customerId, clientId));
        }

        [Fact]
        public async Task Delete_Should_DecreaseCollecitonCount_ByOne()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var result = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            var beforeCollection = await repo.FindAll(customerId, clientId);
            // Act
            await repo.Delete(result.Id, customerId, clientId);
            // Assert 
            var afterCollection = await repo.FindAll(customerId, clientId);
            Assert.True(afterCollection.Count == beforeCollection.Count - 1);
        }

        [Fact]
        public async Task Delete_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Delete(Guid.NewGuid(), customerId, clientId));
        }

        [Fact]
        public async Task Delete_Should_ThrowException_WhenCustomerId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.Empty;
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Delete(Guid.NewGuid(), customerId, clientId));
        }

        [Fact]
        public async Task Update_Should_PersistModifiedProperty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var obj = new MockCustomerEntity { TestField = "BeforeValue" };
            var entity = await repo.Create(obj, customerId, clientId);
            entity.TestField = "NewValue";
            // Act
            var result = await repo.Update(entity, customerId, clientId);
            var after = await repo.FindById(entity.Id, customerId, clientId);
            // Assert
            Assert.Equal("NewValue", after.TestField);
        }

        [Fact]
        public async Task Update_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Update(new MockCustomerEntity(), customerId, clientId));
        }

        [Fact]
        public async Task Update_Should_ThrowException_WhenCustomerId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.Empty;
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Update(new MockCustomerEntity(), customerId, clientId));
        }

        [Fact]
        public async Task FindAll_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.FindAll(customerId, clientId));
        }

        [Fact]
        public async Task FindAll_Should_ThrowException_WhenCustomerId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.Empty;
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.FindAll(customerId, clientId));
        }

        [Fact]
        public async Task FindById_Should_ReturnNull_When_ClientId_NotMatching_Result()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            var wrongClientId = Guid.NewGuid();
            // Act
            var result = await repo.FindById(entity.Id, customerId, wrongClientId);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindById_Should_ReturnNull_When_CustomerId_NotMatching_Result()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var entity = await repo.Create(new MockCustomerEntity(), customerId, clientId);
            var wrongCustomerId = Guid.NewGuid();
            // Act
            var result = await repo.FindById(entity.Id, wrongCustomerId, clientId);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindById_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.Empty;
            var customerId = Guid.NewGuid();
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.FindById(Guid.NewGuid(), customerId, clientId));
        }

        [Fact]
        public async Task FindById_Should_ThrowException_WhenCustomerId_IsEmpty()
        {
            // Arrange
            var repo = new MockCustomerDataRepo<MockCustomerEntity>(_settings);
            var clientId = Guid.NewGuid();
            var customerId = Guid.Empty;
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.FindById(Guid.NewGuid(), customerId, clientId));
        }
    }
}
