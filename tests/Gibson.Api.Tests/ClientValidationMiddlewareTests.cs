using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Gibson.Api;
using Xunit;

namespace Gibson.Api.Tests
{
    public class ClientValidationMiddlewareTests
    {
        private const string NMJ_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImVhYzFlY2JlLTY3YTItNGJmMC1hZTkyLWY3OTJiNjgzYjIxYyIsIkdpYnNvbi1DbGllbnRLZXkiOiJubWoiLCJHaWJzb24tQ2xpZW50SWQiOiIwOTAxY2U0Ni1iYzllLTRkZTctYTJiZi1iODFiODdlNDdmNjIiLCJuYmYiOjE1NTEyMTE0OTksImV4cCI6MTU1MTIxNTA5OSwiaWF0IjoxNTUxMjExNDk5fQ.ls7TmbLWFiAn1a1reDB_znAn-MDVwMupaoWjfWSWQIA";
        private const string CARSHOP_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImVhYzFlY2JlLTY3YTItNGJmMC1hZTkyLWY3OTJiNjgzYjIxYyIsIkdpYnNvbi1DbGllbnRLZXkiOiJjYXJzaG9wIiwiR2lic29uLUNsaWVudElkIjoiMDkwMWNlNDYtYmM5ZS00ZGU3LWEyYmYtYjgxYjg3ZTQ3ZjYyIiwibmJmIjoxNTUxMjExNDk5LCJleHAiOjE1NTEyMTUwOTksImlhdCI6MTU1MTIxMTQ5OX0.4YT-KSSUtUZXyDoEZ1qEibja7zKi1teVwkZYVkDrkNM";

        [Fact]
        public async Task Validation_Should_ThrowException_OnNotMatching_RefererVSHeader_ClientKey()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => throw new NotImplementedException());
            var context = new DefaultHttpContext();
            context.Request.Headers["Gibson-ClientKey"] = "carshop";
            context.Request.Headers["referer"] = "https://mycars.io/nmj/someroute/someotherroute";
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.Invoke(context));
        }

        [Fact]
        public async Task Validation_Should_ThrowException_OnNotMatching_RefererVSToken_ClientKey()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => throw new NotImplementedException());
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer " + CARSHOP_TOKEN;
            context.Request.Headers["Gibson-ClientKey"] = "nmj";
            context.Request.Headers["referer"] = "https://mycars.io/nmj/someroute/someotherroute";
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.Invoke(context));
        }

        [Fact]
        public async Task Validation_Should_ThrowException_OnNotMatching_HeaderVSToken_ClientKey()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => throw new NotImplementedException());
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer " + CARSHOP_TOKEN;
            context.Request.Headers["Gibson-ClientKey"] = "nmj";
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await sut.Invoke(context));
        }

        [Fact]
        public async Task Validation_Should_NotThrowException_OnMatching_RefererVSHeader_ClientKey()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => Task.FromResult(new DefaultHttpContext()));
            var context = new DefaultHttpContext();
            context.Request.Headers["Gibson-ClientKey"] = "nmj";
            context.Request.Headers["referer"] = "https://mycars.io/nmj/someroute/someotherroute";
            // Act & Assert does not throw exception
            await sut.Invoke(context);
        }

        [Fact]
        public async Task Validation_Should_NotThrowException_OnMatching_RefererVSToken_ClientKey()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => Task.FromResult(new DefaultHttpContext()));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer " + NMJ_TOKEN;
            context.Request.Headers["referer"] = "https://mycars.io/nmj/someroute/someotherroute";
            // Act & Assert does not throw exception
            await sut.Invoke(context);
        }

        [Fact]
        public async Task Validation_Should_NotThrowException_OnMatching_HeaderVSToken_ClientKey()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => Task.FromResult(new DefaultHttpContext()));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer " + NMJ_TOKEN;
            context.Request.Headers["Gibson-ClientKey"] = "nmj";

            // Act & Assert does not throw exception
            await sut.Invoke(context);
        }

        [Fact]
        public async Task Validation_Should_NotThrowException_OnMatching_GraphQLUI()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => Task.FromResult(new DefaultHttpContext()));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer " + NMJ_TOKEN;
            context.Request.Headers["referer"] = "https://mycars.io/ui/graphql";

            // Act & Assert does not throw exception
            await sut.Invoke(context);
        }

        [Fact]
        public async Task Validation_Should_NotThrowException_OnMatching_Swagger()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => Task.FromResult(new DefaultHttpContext()));
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer " + NMJ_TOKEN;
            context.Request.Headers["referer"] = "https://mycars.io/swagger/something";

            // Act & Assert does not throw exception
            await sut.Invoke(context);
        }

        [Fact]
        public async Task Validation_Should_SetHeader_WhenValidReferer_WithNoHeaderSet()
        {
            // Arrange
            var sut = new ClientValidationMiddleware((innerHttpContext) => Task.FromResult(new DefaultHttpContext()));
            var context = new DefaultHttpContext();
            context.Request.Headers["referer"] = "https://mycars.io/nmj/something";
            // Act
            await sut.Invoke(context);
            // Assert
            var clientKey = context.Request.Headers["Gibson-ClientKey"];
            Assert.Equal("nmj", clientKey.ToString());
        }
    }
}
