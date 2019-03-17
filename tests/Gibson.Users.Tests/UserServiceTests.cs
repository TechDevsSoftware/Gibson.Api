using System;
using System.Threading.Tasks;
using Gibson.Common.Models;
using Gibson.Tests.Common;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gibson.Users.Tests
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
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.FindByUsername(null, GibsonUserType.Customer, Guid.NewGuid()));
        }

        [Fact]
        public async Task FindByUsername_Should_ThrowException_OnEmptyClientId()
        {
            // Arrange
            var repo = GetMockRepo();
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.FindByUsername("FakeUsername",GibsonUserType.Customer, Guid.Empty));
        }

        [Fact]
        public async Task FindByUsername_Should_ReturnUserWithSameUsername()
        {
            // Arrange
            const string username = "DUMMYUSER";
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Customer }, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByUsername(username, GibsonUserType.Customer, clientId);
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
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Customer  }, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByUsername(username, GibsonUserType.Customer, clientId);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<User>(result);
        }
        
        [Fact]
        public async Task FindByUsername_Customer_Should_Return_Customer()
        {
            // Arrange
            const string username = "DUMMYUSER";
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Customer  }, clientId);
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Employee  }, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByUsername(username, GibsonUserType.Customer, clientId);
            // Assert
            Assert.Equal(GibsonUserType.Customer, result.UserType);
        }
        
        [Fact]
        public async Task FindByUsername_Employee_Should_Return_Employee()
        {
            // Arrange
            const string username = "DUMMYUSER";
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Customer  }, clientId);
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Employee  }, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByUsername(username, GibsonUserType.Employee, clientId);
            // Assert
            Assert.Equal(GibsonUserType.Employee, result.UserType);
        }

        [Fact]
        public async Task FindById_Should_ThrowException_OnEmptyId()
        {
            // Arrange
            var sut = new UserService(GetMockRepo());
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() => await sut.FindById(Guid.Empty, Guid.NewGuid()));
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

        [Fact]
        public async Task UpdateUserProfile_Should_ReturnUpdatedUser()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            var user = new User { Username = "test@test.com" };
            await repo.Create(user, clientId);
            var userProfile = new UserProfile
            {
                FirstName = "FirstName", 
                LastName = "LastName", 
                Email = user.Username, 
                HomePhone = "01525854854", 
                MobilePhone = "07514311311"
            };
            var sut = new UserService(repo);
            // Act
            var result = await sut.UpdateUserProfile(user.Id, userProfile, clientId);
            // Assert
            Assert.Equal(result.UserProfile.FirstName, userProfile.FirstName);
            Assert.Equal(result.UserProfile.LastName, userProfile.LastName);
            Assert.Equal(result.UserProfile.Email, userProfile.Email);
            Assert.Equal(result.UserProfile.HomePhone, userProfile.HomePhone);
            Assert.Equal(result.UserProfile.MobilePhone, userProfile.MobilePhone);
        }
        
        [Fact]
        public async Task UpdateUserProfile_Should_ThrowException_WhenUserNotFound()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.UpdateUserProfile(Guid.NewGuid(), new UserProfile(), clientId));
        }
        
        [Fact]
        public async Task UpdateUserProfile_Should_ThrowException_WhenUserProfileIsNull()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            var user = new User { Username = "test@test.com" };
            await repo.Create(user, clientId);
            var sut = new UserService(repo);
            // Act
            await Assert.ThrowsAsync<Exception>(async () => await sut.UpdateUserProfile(user.Id, null, clientId));
        }
        
        [Fact]
        public async Task UpdateUserProfile_Should_ThrowException_WhenClientIdIsEmpty()
        {
            // Arrange
            var repo = GetMockRepo();
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.UpdateUserProfile(Guid.NewGuid(), new UserProfile(), Guid.Empty));
        }

        [Fact]
        public async Task DeleteUser_Should_ReturnNull_AfterDeletion()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            var user = new User { Username = "test@test.com" };
            await repo.Create(user, clientId);
            var sut = new UserService(repo);
            // Act
            await sut.DeleteUser(user.Id, clientId);
            // Assert
            var result = await sut.FindById(user.Id, clientId);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUser_Should_ThrowException_WhenClientIdIsEmpty()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            var user = new User { Username = "test@test.com" };
            await repo.Create(user, clientId);
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.DeleteUser(user.Id, Guid.Empty));
        }
        
        [Fact]
        public async Task DeleteUser_Should_ThrowException_WhenUserIdIsEmpty()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.DeleteUser(Guid.Empty, clientId));
        }
        
        
        
         
        [Fact]
        public async Task FindByProviderId_Should_ThrowException_OnMissingUsername()
        {
            // Arrange
            var repo = GetMockRepo();
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.FindByProviderId(null, GibsonUserType.Customer, Guid.NewGuid()));
        }

        [Fact]
        public async Task FindByProviderId_Should_ThrowException_OnEmptyClientId()
        {
            // Arrange
            var repo = GetMockRepo();
            var sut = new UserService(repo);
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.FindByProviderId("FakeUsername",GibsonUserType.Customer, Guid.Empty));
        }

        [Fact]
        public async Task FindByProviderId_Should_ReturnUserWithSameUsername()
        {
            // Arrange
            const string username = "DummyUser";
            const string providerId = "123456";
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            await repo.Create(new User
            {
                Username = username, 
                UserType = GibsonUserType.Customer, 
                AuthProfile = new AuthProfile
                {
                    ProviderId = providerId,
                    AuthProvider = GibsonAuthProvider.Google
                }
            }, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByUsername(username, GibsonUserType.Customer, clientId);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<User>(result);
            Assert.Equal(username, result.Username);    
        }
        
        [Fact]
        public async Task FindByProviderId_Customer_Should_Return_Customer()
        {
            // Arrange
            const string username = "DummyUser";
            const string providerId = "123456";
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Customer, AuthProfile  = new AuthProfile {ProviderId =  providerId}}, clientId);
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Employee , AuthProfile = new AuthProfile {ProviderId = providerId}}, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByProviderId(providerId, GibsonUserType.Customer, clientId);
            // Assert
            Assert.Equal(GibsonUserType.Customer, result.UserType);
        }
        
        [Fact]
        public async Task FindByProviderId_Employee_Should_Return_Employee()
        {
            // Arrange
            const string username = "DummyUser";
            const string providerId = "123456";
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Customer, AuthProfile  = new AuthProfile {ProviderId =  providerId}}, clientId);
            await repo.Create(new User { Username = username, UserType = GibsonUserType.Employee , AuthProfile = new AuthProfile {ProviderId = providerId}}, clientId);
            var sut = new UserService(repo);
            // Act
            var result = await sut.FindByProviderId(providerId, GibsonUserType.Employee, clientId);
            // Assert
            Assert.Equal(GibsonUserType.Employee, result.UserType);
        }

        
    }
}
