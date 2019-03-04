using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using TechDevs.Gibson.Api;
using Xunit;

namespace Gibson.Api.Tests
{
    public class GibsonApiExtentionHelperTests
    {
        private const string NMJ_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImVhYzFlY2JlLTY3YTItNGJmMC1hZTkyLWY3OTJiNjgzYjIxYyIsIkdpYnNvbi1DbGllbnRLZXkiOiJubWoiLCJHaWJzb24tQ2xpZW50SWQiOiIwOTAxY2U0Ni1iYzllLTRkZTctYTJiZi1iODFiODdlNDdmNjIiLCJuYmYiOjE1NTEyMTE0OTksImV4cCI6MTU1MTIxNTA5OSwiaWF0IjoxNTUxMjExNDk5fQ.ls7TmbLWFiAn1a1reDB_znAn-MDVwMupaoWjfWSWQIA";
        private const string CARSHOP_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImVhYzFlY2JlLTY3YTItNGJmMC1hZTkyLWY3OTJiNjgzYjIxYyIsIkdpYnNvbi1DbGllbnRLZXkiOiJjYXJzaG9wIiwiR2lic29uLUNsaWVudElkIjoiMDkwMWNlNDYtYmM5ZS00ZGU3LWEyYmYtYjgxYjg3ZTQ3ZjYyIiwibmJmIjoxNTUxMjExNDk5LCJleHAiOjE1NTEyMTUwOTksImlhdCI6MTU1MTIxMTQ5OX0.4YT-KSSUtUZXyDoEZ1qEibja7zKi1teVwkZYVkDrkNM";

        [Fact]
        public void GetClientKeyFromUri_Should_OnValidInput_NotReturn_NullOrEmptyString()
        {
            // Arrange
            Uri uri = new Uri(@"https://mycars.io/nmj/someroute/someotherroute");
            // Act
            var res = uri.GetClientKeyFromUri();
            // Assert
            Assert.False(string.IsNullOrEmpty(res));
        }

        [Fact]
        public void GetClientKeyFromUri_Should_OnValidInput_ReturnCorrectValue()
        {
            // Arrange
            Uri uri = new Uri(@"https://mycars.io/nmj/someroute/someotherroute");
            // Act
            var res = uri.GetClientKeyFromUri();
            // Assert
            Assert.Equal("nmj", res);
        }

        [Fact]
        public void GetClientKeyFromUri_Should_OnInvalidInput_ReturnNull()
        {
            // Arrange
            Uri uri = new Uri(@"https://mycars.io");
            // Act
            var res = uri.GetClientKeyFromUri();
            // Assert
            Assert.Null(res);
        }

        [Fact]
        public void GetClientKeyFromUri_Should_ReturnNull_OnNull()
        {
            // Arrange
            Uri uri = new Uri(@"https://mycars.io");
            // Act
            var res = uri.GetClientKeyFromUri();
            // Assert
            Assert.Null(res);
        }

        [Fact]
        public void GetTokenFromRequest_Should_OnValidInput_ReturnValidJwtToken()
        {
            // Arrange 
            var ctx = new DefaultHttpContext();
            ctx.Request.Headers["Authorization"] = "Bearer " + NMJ_TOKEN;
            // Act
            var res = ctx.Request.GetTokenFromRequest();
            // Assert
            var handler = new JwtSecurityTokenHandler();
            var canRead = handler.CanReadToken(res);
            Assert.True(canRead);
        }
    }
}
