using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Gibson.AuthTokens
{
    public class AuthTokenServiceTests
    {
        private const string SECRET =
            "TechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKey";

        [Fact]
        public void CreateToken_Should_ReturnReadableJwt()
        {
            // Arrange
            var sut = new AuthTokenService();
            // Act
            var result = sut.CreateToken(Guid.NewGuid(), "Key", Guid.NewGuid());
            // Assert
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(result));
        }

        [Fact]
        public void CreateToken_Should_HaveUserIdClaim()
        {
            // Arrange
            var sut = new AuthTokenService();
            var userId = Guid.NewGuid();
            // Act
            var result = sut.CreateToken(userId, "Key", Guid.NewGuid());
            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result);
            var value = jwt.Claims.First(x => x.Type == "unique_name").Value;
            Assert.Equal(userId.ToString(), value);
        }

        [Fact]
        public void CreateToken_Should_HaveClientKeyClaim()
        {
            // Arrange
            var sut = new AuthTokenService();
            var clientKey = "ClientKey";
            // Act
            var result = sut.CreateToken(Guid.NewGuid(), clientKey, Guid.NewGuid());
            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result);
            var value = jwt.Claims.First(x => x.Type == "Gibson-ClientKey").Value;
            Assert.Equal(clientKey, value);
        }

        [Fact]
        public void CreateToken_Should_HaveClientIdClaim()
        {
            // Arrange
            var sut = new AuthTokenService();
            var clientId = Guid.NewGuid();
            // Act
            var result = sut.CreateToken(Guid.NewGuid(), "Key", clientId);
            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result);
            var value = jwt.Claims.First(x => x.Type == "Gibson-ClientId").Value;
            Assert.Equal(clientId.ToString(), value);
        }

        [Fact]
        public void CreateToken_Should_ThrowException_OnEmptyUserId()
        {
            // Arrange
            var sut = new AuthTokenService();
            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.CreateToken(Guid.Empty, "NMJ", Guid.NewGuid()));
        }

        [Fact]
        public void CreateToken_Should_ThrowException_OnEmptyClientId()
        {
            // Arrange
            var sut = new AuthTokenService();
            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.CreateToken(Guid.NewGuid(), "NMJ", Guid.Empty));
        }

        [Fact]
        public void CreateToken_Should_ThrowException_OnEmptyClientKey()
        {
            // Arrange
            var sut = new AuthTokenService();
            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.CreateToken(Guid.NewGuid(), "", Guid.Empty));
        }

        [Fact]
        public void CreateToken_Should_ThrowException_OnNullClientKey()
        {
            // Arrange
            var sut = new AuthTokenService();
            // Act & Assert
            Assert.Throws<ArgumentException>(() => sut.CreateToken(Guid.NewGuid(), "", Guid.Empty));
        }

        [Fact]
        public void CreateToken_Should_ReturnNotExpiredToken()
        {
            // Arrange
            var sut = new AuthTokenService();
            // Act
            var result = sut.CreateToken(Guid.NewGuid(), "NMJ", Guid.NewGuid());
            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result);
            Assert.True(token.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void CreateToken_Should_ProduceAValidateableToken()
        {
            // Arrange
            var sut = new AuthTokenService(SECRET);
            // Act
            var result = sut.CreateToken(Guid.NewGuid(), "NMJ", Guid.NewGuid());
            // Assert
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SECRET);
            var validationParams = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RequireSignedTokens = true,
                ValidateAudience = false,
                ValidateIssuer = false
            };
            handler.ValidateToken(result, validationParams, out var validatedToken);
        }

        [Fact]
        public void ValidateToken_Should_ReturnFalse_OnUnreadableToken()
        {
            // Arrange
            var invalidToken = @"ThisIsAnUnreadableToken";
            var sut = new AuthTokenService();
            // Act
            var result = sut.ValidateToken(invalidToken);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateToken_Should_ReturnTrue_OnGoodToken()
        {
            // Arrange
            var invalidToken =
                @"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjY5NGYxODE5LTljMWMtNDYyMS1hN2JhLTQwMDYyNjBiYmI4YyIsIkdpYnNvbi1DbGllbnRLZXkiOiJubWoiLCJHaWJzb24tQ2xpZW50SWQiOiIwOTAxY2U0Ni1iYzllLTRkZTctYTJiZi1iODFiODdlNDdmNjIiLCJuYmYiOjE1NTIwNTc2MzMsImV4cCI6MTU1MjA2MTIzMywiaWF0IjoxNTUyMDU3NjMzfQ.JLkdfKRZGXDtbueqduvaXYMmYeSV3fgGmcNXqO1K7b4";
            var sut = new AuthTokenService();
            // Act
            var result = sut.ValidateToken(invalidToken);
            // Assert
            Assert.True(result);
        }
    }
}