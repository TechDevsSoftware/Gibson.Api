using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Gibson.Auth.Crypto;
using Gibson.Auth.Tokens;
using Gibson.Clients;
using Gibson.Common.Enums;
using Gibson.Common.Models;
using Gibson.Tests.Common;
using Gibson.Users;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gibson.Auth.Tests
{
    public class AuthServiceTests: IClassFixture<DatabaseTestFixture>, IClassFixture<MockClientServiceFixture>
    {
        private readonly IUserRepository _repo;
        private readonly IClientRepository _clientRepo;
        private readonly IPasswordHasher _hasher;
        private readonly Client _dummyClient;

        public AuthServiceTests(DatabaseTestFixture fixture)
        {
            var connString = fixture.Db.ConnectionString;
            var dbSettings = new MongoDbSettings { ConnectionString = connString, Database = "Testing" };
            var settings = Options.Create(dbSettings);
            _hasher = new BCryptPasswordHasher();
            _repo = new UserRepository("DummyUsers", settings);
            _clientRepo = new ClientRepository(settings);
            _dummyClient = new Client
            {
                Id = Guid.NewGuid().ToString(),
                Name =  "Dummy",
                ShortKey = "dummy"
            };
        }
        
        private AuthService GetAuthService()
        {
            var userService = new UserService(_repo);
            var tokenService = new AuthTokenService();
            var clientService = new ClientService(_clientRepo);
            var service = new AuthService(tokenService, userService, _hasher, clientService);
            return service;
        }
        
        [Fact]
        public async Task Login_Should_ReturnReadableToken_OnValidGibsonRequest()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var password = "PASSW0RD";
            var hash = _hasher.HashPassword(password);
            var user = new User
            {
                Username = "user@email.com",
                UserType = GibsonUserType.Customer,
                AuthProfile = new AuthProfile
                {
                    AuthProvider = GibsonAuthProvider.Gibson,
                    PasswordHash = hash
                }
            };
            await _repo.Create(user, clientId);
            var sut = GetAuthService();
            var req = new LoginRequest
            {
                Email =  user.Username,
                Password = password
            };
            // Act
            var result = await sut.Login(req, GibsonUserType.Customer, clientId);
            // Assert
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(result));
        }
        
        [Fact]
        public async Task Login_Should_ThrowException_WhenUnsupportedProvider()
        {
            // Arrange
            var sut = GetAuthService();
            var req = new LoginRequest
            {
                Email =  "user@email.com",
                Password = "CorrectPassword",
            };
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.Login(req, GibsonUserType.Customer, Guid.NewGuid()));
        }
        
        [Fact]
        public async Task Login_Should_ThrowException_WhenUserIsNotFound()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var password = "PASSW0RD";
            var hash = _hasher.HashPassword(password);
            var user = new User
            {
                Username = "user@email.com",
                UserType = GibsonUserType.Customer,
                AuthProfile = new AuthProfile
                {
                    AuthProvider = GibsonAuthProvider.Gibson,
                    PasswordHash = hash
                }
            };
            await _repo.Create(user, clientId);
            var sut = GetAuthService();
            var req = new LoginRequest
            {
                Email =  "NotAUser@email.com",
                Password = password
            };
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.Login(req, GibsonUserType.Customer, clientId) );
        }
        
        [Fact]
        public async Task Login_Should_ThrowException_PasswordIsIncorrect()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var password = "PASSW0RD";
            var hash = _hasher.HashPassword(password);
            var user = new User
            {
                Username = "user@email.com",
                UserType = GibsonUserType.Customer,
                AuthProfile = new AuthProfile
                {
                    AuthProvider = GibsonAuthProvider.Gibson,
                    PasswordHash = hash
                }
            };
            await _repo.Create(user, clientId);
            var sut = GetAuthService();
            var req = new LoginRequest
            {
                Email =  user.Username,
                Password = "WrongPassword"
            };
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.Login(req, GibsonUserType.Customer, clientId));
        }

        [Fact]
        public async Task Login_Should_ThrowException_When_UserType_IsNotSet()
        {
            // Arrange
            var req = new LoginRequest
            {
                Email = "user@email.com"
            };
            var sut = GetAuthService();
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Login(req, GibsonUserType.Customer, Guid.NewGuid()));
        }
        
        [Fact]
        public async Task Login_Should_ThrowException_When_Provider_IsNotSet()
        {
            // Arrange
            var req = new LoginRequest
            {
                Email = "user@email.com",
            };
            var sut = GetAuthService();
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Login(req, GibsonUserType.Customer, Guid.NewGuid()));
        }

        [Fact]
        public async Task Login_Should_CreateNewAuthEvent()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var password = "PASSW0RD";
            var hash = _hasher.HashPassword(password);
            var user = new User
            {
                Username = "user@email.com",
                UserType = GibsonUserType.Customer,
                AuthProfile = new AuthProfile
                {
                    AuthProvider = GibsonAuthProvider.Gibson,
                    PasswordHash = hash
                }
            };
            await _repo.Create(user, clientId);
            var sut = GetAuthService();
            var req = new LoginRequest
            {
                Email =  user.Username,
                Password = password
            };
            // Act
            var result = await sut.Login(req, GibsonUserType.Customer, clientId);
            // Assert
            var userResult = await _repo.GetUserByUserName(user.Username, user.UserType, clientId);
            var loginEvent =
                userResult.AuthProfile.AuthEvents.Where(x => x.EventType == GibsonAuthEventType.SuccessfulLogin);
            Assert.True(loginEvent.Any());
        }

    }

    public class MockClientServiceFixture
    {
    }
}
