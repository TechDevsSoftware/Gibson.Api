using System;
using System.Threading.Tasks;
using Gibson.Shared.Repositories.Tests;
using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using Xunit;

namespace Gibson.Users
{
    public class UserServiceTests : IClassFixture<DatabaseTestFixture>
    {
        private readonly IOptions<MongoDbSettings> _settings;

        public UserServiceTests(DatabaseTestFixture fixture)
        {
            var connString = fixture.Db.ConnectionString;
            var dbSettings = new MongoDbSettings { ConnectionString = connString, Database = "Testing" };
            _settings = Options.Create(dbSettings);
        }

        private IUserRepository GetMockRepo() => new UserRepository("DummyUsers", _settings); 
        
        [Fact]
        public async Task FindByUsername_Should_ThrowException_OnMissingUsername()
        {
            // Arrange
            var repo = GetMockRepo();
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.FindByUsername(null, Guid.NewGuid()));
        }

        [Fact]
        public async Task FindByUsername_Should_ThrowException_OnEmptyClientId()
        {
            // Arrange
           var repo = GetMockRepo();
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.FindByUsername("FakeUsername", Guid.Empty));
        }

        [Fact]
        public async Task FindByUsername_Should_ReturnUserWithSameUsername()
        {
            // Arrange
            const string username = "DUMMYUSER";
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            await repo.Create(new User { Username = username }, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByUsername(username, clientId);
            // Assert
            Assert.Equal(username, result.Username);    
        }

        [Fact]
        public async Task FindByUsername_Should_ReturnNotNull_ObjectOfTypeUser()
        {
            // Arrange
            const string username = "DUMMYUSER";
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            await repo.Create(new User { Username = username }, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByUsername(username, clientId);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<User>(result);
        }

        [Fact]
        public async Task FindById_Should_ThrowException_OnEmptyId()
        {
            // Arrange
            var sut = new UserService(GetMockRepo());
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async() => await sut.FindById(Guid.Empty, Guid.NewGuid()));
        }

        [Fact]
        public async Task FindById_Should_ThrowException_OnEmptyClientId()
        {
            // Arrange
            var sut = new UserService(GetMockRepo());
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.FindById(Guid.NewGuid(), Guid.Empty));
        }
        
        [Fact]
        public async Task FindByUserId_Should_ReturnNotNull_ObjectOfTypeUser()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            var user = new User();
            await repo.Create(user, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindById(user.Id, clientId);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<User>(result);
        }
    }
}
