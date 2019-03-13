using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Gibson.AuthTokens;
using Gibson.Shared.Repositories.Tests;
using Gibson.Users;
using Microsoft.Extensions.Options;
using TechDevs.Shared;
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

//        [Fact]
//        public async Task Login_Should_ReturnReadableToken_OnValidGoogleRequest()
//        {
//            // Arrange
//            var clientId = Guid.NewGuid();
//            var user = new User
//            {
//                Username = "user@email.com",
//                UserType = GibsonUserType.Customer,
//                AuthProfile = new AuthProfile
//                {
//                    AuthProvider = GibsonAuthProvider.Google,
//                    ProviderId = "107387075381855787656"
//                }
//            };
//            await _repo.Create(user, clientId);
//            var sut = GetAuthService();
//            var req = new LoginRequest
//            {
//                Email =  user.Username,
//                Provider = "Google",
//                ProviderIdToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImNmMDIyYTQ5ZTk3ODYxNDhhZDBlMzc5Y2M4NTQ4NDRlMzZjM2VkYzEiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJhY2NvdW50cy5nb29nbGUuY29tIiwiYXpwIjoiNTYyNDAwODAyNTEzLWw4MW03NHRkNDVtNDNtM3I4bWo5cXEzaXBkZzlvMDcxLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiYXVkIjoiNTYyNDAwODAyNTEzLWw4MW03NHRkNDVtNDNtM3I4bWo5cXEzaXBkZzlvMDcxLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTA3Mzg3MDc1MzgxODU1Nzg3NjU2IiwiaGQiOiJ0ZWNoZGV2cy5uZXQiLCJlbWFpbCI6InN0ZXZlQHRlY2hkZXZzLm5ldCIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJhdF9oYXNoIjoiZHQ3ZDdvZ0tRa01KMzYtdnJtY0cwZyIsIm5hbWUiOiJTdGV2ZSBLZW50IiwicGljdHVyZSI6Imh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS8tTFlITS1XNlp1OXMvQUFBQUFBQUFBQUkvQUFBQUFBQUFBQUEvQUNIaTNyZUdxMFpSVGJtUW9XV1kyQWo4am9kWlJJTW9rQS9zOTYtYy9waG90by5qcGciLCJnaXZlbl9uYW1lIjoiU3RldmUiLCJmYW1pbHlfbmFtZSI6IktlbnQiLCJsb2NhbGUiOiJlbiIsImlhdCI6MTU1MTc5MzI4OCwiZXhwIjoxNTUxNzk2ODg4LCJqdGkiOiI3YWJhNzg4ZGFkNDY4MWVkMDliMmU2YTRhMzFhZWUzZTIyOWQ5YTdiIn0.LjUrbCBdAUkfZLBWtWcgfOZkhcQVoTcP02tpDYAf_fqQVpZ5cYBmKbuLV7p-nnZKh8WBfPyWYUmzUmnyjwBROkDVgSDhu8TP8Zr1AEwTTDvakEWSDqfddS8HdTHh9tfNqP88OTFtwyLA5k1W52XhsB7iKRt4KWMwBMvxnIiU2aZvW-IwyH9yfJ-W3SWKvr8HufKQsWCscqhYDjfOlXfOXPOqeYgY9fBsLwiJ8uyOQSgL4-uuSlKv1_x-cZfbAzHhcHwjqfCUNV8Nmz0i5o4-Fqb0lxoeCtn1u96vr0wOHQ4Oa_ku2ZTHW8d60GZrXTXpIh7PL79BgxnBhTBfbq7c4A",
//                ClientId = clientId,
//                ClientKey = "nmj",
//                UserType = GibsonUserType.Customer
//            };
//            // Act
//            var result = await sut.Login(req);
//            // Assert
//            var handler = new JwtSecurityTokenHandler();
//            Assert.True(handler.CanReadToken(result));
//        }
        
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
                ClientKey = "nmj",
                UserType = GibsonUserType.Customer,
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
                ClientKey = "nmj",
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
                ClientKey = "nmj",
                UserType = GibsonUserType.Customer
            };
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.Login(req));
        }

        [Fact]
        public async Task Login_Should_ThrowException_When_UserType_IsNotSet()
        {
            // Arrange
            var req = new LoginRequest
            {
                Email = "user@email.com",
                Provider =  "Gibson",
                ClientId = Guid.NewGuid(),
                ClientKey = "nmj"
            };
            var sut = GetAuthService();
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Login(req));
        }
        
        [Fact]
        public async Task Login_Should_ThrowException_When_Provider_IsNotSet()
        {
            // Arrange
            var req = new LoginRequest
            {
                Email = "user@email.com",
                ClientId = Guid.NewGuid(),
                ClientKey = "nmj",
                UserType = GibsonUserType.Customer
            };
            var sut = GetAuthService();
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Login(req));
        }
     
    }
}
