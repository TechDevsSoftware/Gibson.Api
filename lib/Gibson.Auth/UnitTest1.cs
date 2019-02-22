using System;
using System.IdentityModel.Tokens.Jwt;
using Gibson.AuthTokens;
using TechDevs.Shared.Models;
using Xunit;

namespace Gibson.Auth
{
    public class AuthServiceTests
    {
        [Fact]
        public void Login_Should_ReturnReadableToken()
        {
            // Arrange
            var tokenService = new AuthTokenService();
            var sut = new AuthService(tokenService);
            var req = new LoginRequest { Email = "user@email.com", Password = "CorrectPassword", Provider = "MyCars" };
            // Act
            var result = sut.Login(req);
            // Assert
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(result));
        }
    }

    public class AuthService
    {
        private readonly IAuthTokenService tokenService;

        public AuthService(IAuthTokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        public string Login(LoginRequest req)
        {
            return tokenService.CreateToken(Guid.NewGuid(), "NMJ", Guid.NewGuid());
        }
    }
}
