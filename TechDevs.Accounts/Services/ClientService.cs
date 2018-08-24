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

        public async Task<List<Client>> GetClients() => await _clientRepo.GetClients();
        public async Task<Client> GetClient(string clientId, bool includeRelatedAuthUsers) => await _clientRepo.GetClient(clientId, includeRelatedAuthUsers);
        public async Task<Client> CreateClient(ClientRegistration reg)
        {
            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                Name = reg.Name,
                SiteUrl = reg.SiteUrl,
                ClientApiKey = Guid.NewGuid().ToString()
            };
            return await _clientRepo.CreateClient(client);
        }
        public async Task<Client> DeleteClient(string clientId) => await _clientRepo.DeleteClient(clientId);
        public async Task<Client> UpdateClient(string propertyPath, List<Type> data, string clientId) => await _clientRepo.UpdateClient(propertyPath, data, clientId);
    }
}
