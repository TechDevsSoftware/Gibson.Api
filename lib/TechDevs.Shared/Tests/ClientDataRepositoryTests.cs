using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using Xunit;

namespace Gibson.Shared.Repositories.Tests
{
    public class ClientDataRepositoryTests : IClassFixture<DatabaseTestFixture>
    {
        private readonly IOptions<MongoDbSettings> _settings;

        public ClientDataRepositoryTests(DatabaseTestFixture fixture)
        {
            var connString = fixture.Db.ConnectionString;
            var dbSettings = new MongoDbSettings { ConnectionString = connString, Database = "Testing" };
            _settings = Options.Create(dbSettings);
        }
        private IRepository<MockClientEntity> GetMockRepo() => new ClientDataRepository<MockClientEntity>("DummyCollection", _settings);

        [Fact]
        public async Task Create_Should_IncreaseCollectionCount_ByOne()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var beforeCollection = await repo.FindAll(clientId);
            // Act
            var result = await repo.Create(new MockClientEntity(), clientId);
            // Assert 
            var afterCollection = await repo.FindAll(clientId);
            Assert.True(afterCollection.Count == beforeCollection.Count + 1);
        }
        
        [Fact]
        public async Task Create_Should_SetClientId()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockClientEntity(), clientId);
            // Assert 
            Assert.Equal(clientId, result.ClientId);
        }

        [Fact]
        public async Task Create_Should_SetId()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockClientEntity(), clientId);
            // Assert 
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task Create_Should_ReuturnClientEntity()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            // Act
            var result = await repo.Create(new MockClientEntity(), clientId);
            // Assert 
            Assert.IsType<MockClientEntity>(result);
        }

        [Fact]
        public async Task Create_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.Empty;
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Create(new MockClientEntity(), clientId));
        }

        [Fact]
        public async Task Delete_Should_DecreaseCollecitonCount_ByOne()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var result = await repo.Create(new MockClientEntity(), clientId);
            var beforeCollection = await repo.FindAll(clientId);
            // Act
            await repo.Delete(result.Id, clientId);
            // Assert 
            var afterCollection = await repo.FindAll(clientId);
            Assert.True(afterCollection.Count == beforeCollection.Count - 1);
        }

        [Fact]
        public async Task Delete_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.Empty;
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Delete(Guid.NewGuid(), clientId));
        }

        [Fact]
        public async Task Update_Should_PersistModifiedProperty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var obj = new MockClientEntity { TestField = "BeforeValue" };
            var entity = await repo.Create(obj, clientId);
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
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.Update(new MockClientEntity(), clientId));
        }

        [Fact]
        public async Task FindAll_Should_ThrowException_WhenClientId_IsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.Empty;
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.FindAll(clientId));
        }

        [Fact]
        public async Task FindById_Should_ReturnNull_When_ClientId_NotMatching_Result()
        {
            // Arrange
            var repo = GetMockRepo();
            var clientId = Guid.NewGuid();
            var entity = await repo.Create(new MockClientEntity(), clientId);
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
            // Act & Assert 
            await Assert.ThrowsAsync<Exception>(async () => await repo.FindById(Guid.NewGuid(), clientId));
        }
    }
}
