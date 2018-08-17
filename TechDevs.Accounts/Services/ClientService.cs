using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Accounts.Repositories;

namespace TechDevs.Accounts.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepo;

        public ClientService(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public async Task<IClient> GetClient(string clientId) => await _clientRepo.GetClient(clientId);
        public async Task<IClient> CreateClient(IClient client) => await _clientRepo.CreateClient(client);
        public async Task<IClient> DeleteClient(string clientId) => await _clientRepo.DeleteClient(clientId);
        public async Task<IClient> UpdateClient(string propertyPath, List<Type> data, string clientId) => await _clientRepo.UpdateClient(propertyPath, data, clientId);
    }
}
