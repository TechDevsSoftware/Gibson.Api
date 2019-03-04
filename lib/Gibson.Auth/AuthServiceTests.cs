using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Gibson.AuthTokens;
using Gibson.Shared.Repositories.Tests;
using Gibson.Users;
using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using Xunit;

namespace Gibson.Auth
{
    public class AuthServiceTests: IClassFixture<DatabaseTestFixture>
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;

        public AuthServiceTests(DatabaseTestFixture fixture)
        {
            var connString = fixture.Db.ConnectionString;
            var dbSettings = new MongoDbSettings { ConnectionString = connString, Database = "Testing" };
            var settings = Options.Create(dbSettings);
            _hasher = new BCryptPasswordHasher();
            _repo = new UserRepository("DummyUsers", settings);
        }
        
        private AuthService GetAuthService()
        {
            var userService = new UserService(_repo);
            var tokenService = new AuthTokenService();
            var service = new AuthService(tokenService, userService, _hasher);
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
                Password = password,
                Provider = "Gibson",
                ClientId = clientId,
                ClientKey = "nmj",
                UserType = GibsonUserType.Customer
            };
            // Act
            var result = await sut.Login(req);
            // Assert
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(result));
        }

        [Fact]
        public async Task Login_Should_ReturnReadableToken_OnValidGoogleRequest()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var user = new User
            {
                Username = "user@email.com",
                UserType = GibsonUserType.Customer,
                AuthProfile = new AuthProfile
                {
                    AuthProvider = GibsonAuthProvider.Google,
                    ProviderId = "1234567"
                }
            };
            await _repo.Create(user, clientId);
            var sut = GetAuthService();
            var req = new LoginRequest
            {
                Email =  user.Username,
                Provider = "Gibson",
                ProviderIdToken = user.AuthProfile.ProviderId,
                ClientId = clientId,
                ClientKey = "nmj",
                UserType = GibsonUserType.Customer
            };
            // Act
            var result = await sut.Login(req);
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
                Provider = "InvalidProvider",
                ClientId = Guid.NewGuid(),
                UserType = GibsonUserType.Customer
            };
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.Login(req));
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
                Password = password,
                Provider = "Gibson",
                ClientId = clientId,
                UserType = GibsonUserType.Customer
            };
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await sut.Login(req));
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
                Password = "WrongPassword",
                Provider = "Gibson",
                ClientId = clientId,
                UserType = GibsonUserType.Customer
            };
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.Login(req));
        }
    }
}
