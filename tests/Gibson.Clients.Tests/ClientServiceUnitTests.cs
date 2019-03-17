using Xunit;
using Moq;
using System.Threading.Tasks;
using System;
using Gibson.Clients;
using Gibson.Common.Models;

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
            mockRepo.Setup(x => x.GetClient(updatingClient.Id)).ReturnsAsync(existingClient);
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
            mockRepo.Setup(x => x.GetClient(updatingClient.Id)).ReturnsAsync(updatingClient);
            var sut = new ClientService(mockRepo.Object);
            // Act
            var result = await sut.UpdateClient(updatingClient.Id, updatingClient);
            // Assert
            Assert.True(result != null);
            Assert.True(result.Id != null);
        }
    }
}
