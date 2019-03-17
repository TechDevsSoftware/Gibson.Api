using System;
using Moq;

namespace Gibson.Clients.Tests
{
    public class MockClientFixture : IDisposable
    {
        public Mock<IClientRepository> ClientRepository;
        
        public MockClientFixture()
        {
            ClientRepository = new Mock<IClientRepository>();       
        }

        public void Dispose()
        {
            
        }
    }
}