using Xunit;
using Moq;
using TechDevs.Shared.Models;
using System.Threading.Tasks;
using System;
using Mongo2Go;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TechDevs.Clients.IntegrationTests
{
    public class ClientServiceIntegrationTests
    {
        private MongoDbRunner _runner;
        private TestServer _testServer;
        private HttpClient _client;

        public ClientServiceIntegrationTests()
        {
            _runner = MongoDbRunner.StartForDebugging();
            _runner.Import("accounts", "Clients", Path.Combine(Directory.GetCurrentDirectory(), @"Client_SeedData.json"), true);
            var connString = _runner.ConnectionString;

            var builder = new WebHostBuilder()
                .UseEnvironment("IntegrationTesting")
                .UseStartup<TechDevs.Gibson.Api.Startup>();

            _testServer = new TestServer(builder);
            _client = _testServer.CreateClient();
        }

        [Theory]
        [InlineData("api/v1/clients/")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var result = await response.Content.ReadAsStringAsync();

            var clients = JsonConvert.DeserializeObject<List<Client>>(result);
            Assert.IsType<List<Client>>(clients);
        }
    }
}

namespace TechDevs.Clients.UnitTests
{

    public class ClientServiceUnitTests
    {
        [Fact]
        public async Task CreateClient_ShouldThrowException_On_DuplicateShortKey()
        {
            // Arrange
            var reg = new ClientRegistration { Name = "TestClient", ShortKey = "KEY1" };
            var mockClient = new Client { Name = "TestClient", ShortKey = "KEY1" };
            var mockRepo = new Mock<IClientRepository>();
            mockRepo.Setup(x => x.GetClientByShortKey(It.IsAny<string>())).ReturnsAsync(mockClient);
            var sut = new ClientService(mockRepo.Object);
            // Act & Assert 
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.CreateClient(reg));
            Assert.True(ex.Message.ToLower() == "short key is already in use");
        }

        [Fact]
        public async Task CreateClient_ShouldReturn_NonNullClient_WithId()
        {
            // Arrange
            var reg = new ClientRegistration { Name = "NewName", ShortKey = "NewKey" };
            var mockClient = new Client { Id = Guid.NewGuid().ToString() };
            var mockRepo = new Mock<IClientRepository>();
            mockRepo.Setup(x => x.GetClientByShortKey(It.IsAny<string>())).ReturnsAsync((Client)null);
            mockRepo.Setup(x => x.CreateClient(It.IsAny<Client>())).ReturnsAsync(mockClient);
            var sut = new ClientService(mockRepo.Object);
            // Act
            var result = await sut.CreateClient(reg);
            // Assert
            Assert.True(result != null);
            Assert.True(result.Id != null);
        }

        [Fact]
        public async Task UpdateClient_ShouldThrowException_On_DuplicateShortKey()
        {
            // Arrange
            var existingClient = new Client { Id = Guid.NewGuid().ToString(), Name = "ExistingKey", ShortKey = "KEY1" };
            var existingClient2 = new Client { Id = Guid.NewGuid().ToString(), Name = "ExistingKey2", ShortKey = "KEY2" };
            var updatingClient = new Client { Id = existingClient.Id, Name = "ModifiedName", ShortKey = existingClient2.ShortKey };
            var mockRepo = new Mock<IClientRepository>();
            mockRepo.Setup(x => x.GetClient(updatingClient.Id, false)).ReturnsAsync(existingClient);
            mockRepo.Setup(x => x.GetClientByShortKey(updatingClient.ShortKey)).ReturnsAsync(existingClient);
            var sut = new ClientService(mockRepo.Object);
            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => sut.UpdateClient(updatingClient.Id, updatingClient));
            Assert.True(ex.Message.ToLower() == "short key is already in use");
        }

        [Fact]
        public async Task UpdateClient_ShouldReturn_NonNullClient_WithId()
        {
            // Arrange
            var updatingClient = new Client { Id = Guid.NewGuid().ToString(), Name = "DuplicateKey", ShortKey = "KEY1" };
            var mockRepo = new Mock<IClientRepository>();
            mockRepo.Setup(x => x.GetClientByShortKey(It.IsAny<string>())).ReturnsAsync((Client)null);
            mockRepo.Setup(x => x.UpdateClient(updatingClient.Id, updatingClient)).ReturnsAsync(updatingClient);
            mockRepo.Setup(x => x.GetClient(updatingClient.Id, false)).ReturnsAsync(updatingClient);
            var sut = new ClientService(mockRepo.Object);
            // Act
            var result = await sut.UpdateClient(updatingClient.Id, updatingClient);
            // Assert
            Assert.True(result != null);
            Assert.True(result.Id != null);
        }
    }
}
