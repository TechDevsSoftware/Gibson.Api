using Xunit;
using TechDevs.Shared.Models;
using System.Threading.Tasks;
using Mongo2Go;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.IO;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Formatting;
using Gibson.Shared.Repositories.Tests;
using Microsoft.Extensions.Configuration;

namespace TechDevs.Clients.IntegrationTests
{
    public class ApiE2E : IClassFixture<DatabaseTestFixture>
    {
        private const string NMJ_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImVhYzFlY2JlLTY3YTItNGJmMC1hZTkyLWY3OTJiNjgzYjIxYyIsIkdpYnNvbi1DbGllbnRLZXkiOiJubWoiLCJHaWJzb24tQ2xpZW50SWQiOiIwOTAxY2U0Ni1iYzllLTRkZTctYTJiZi1iODFiODdlNDdmNjIiLCJuYmYiOjE1NTEyMTE0OTksImV4cCI6MTU1MTIxNTA5OSwiaWF0IjoxNTUxMjExNDk5fQ.ls7TmbLWFiAn1a1reDB_znAn-MDVwMupaoWjfWSWQIA";

        private MongoDbRunner _runner;
        private TestServer _testServer;
        private HttpClient _client;

        public ApiE2E(DatabaseTestFixture fixture)
        {
            _runner = fixture.Db;

            var dbSettings = new MongoDbSettings { ConnectionString = _runner.ConnectionString, Database = "Testing" };

            _runner.Import(dbSettings.Database, "Clients", Path.Combine(Directory.GetCurrentDirectory(), @"Client_SeedData.json"), true);
            _runner.Import(dbSettings.Database, "AuthUsers", Path.Combine(Directory.GetCurrentDirectory(), @"Users_SeedData.json"), true);

            var settings = new Dictionary<string, string>()
            {
                { "MongoDbSettings:ConnectionString", dbSettings.ConnectionString },
                { "MongoDbSettings:Database", dbSettings.Database }
            };

            var builder = new WebHostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) => config.AddInMemoryCollection(settings))
                .UseStartup<Gibson.Api.Startup>();

            _testServer = new TestServer(builder);
            _client = _testServer.CreateClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + NMJ_TOKEN);
            _client.DefaultRequestHeaders.Add("Gibson-ClientKey", "nmj");
            _client.DefaultRequestHeaders.Add("referer", "https://mycars.io/nmj/route/anotherroute");
        }

        [Theory]
        [InlineData("api/v1/clients/")]
        [InlineData("api/v1/clients/3061cd9e-f525-4af9-b906-0a78fa8355ec")]
        public async Task Get_ClientList_ReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange & Act
            var response = await _client.GetAsync(url);
            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task EmployeeLogin_Returns_JwtToken_OnValidLogin()
        {
            // Arrange
            var url = "api/v1/employees/login";
            var body = new LoginRequest { Email = "test@test.com", Password = "test", ClientKey = "nmj", Provider = "TechDevs" };
            var handler = new JwtSecurityTokenHandler();
            // Act
            var response = await _client.PostAsync(url, body, new JsonMediaTypeFormatter());
            // Assert
            Assert.True(response.IsSuccessStatusCode);
            var token = await response.Content.ReadAsStringAsync();
            Assert.True(handler.CanReadToken(token));
        }
    }
}
