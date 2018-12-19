using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<Client> GetClientByShortKey(string key) => await _clientRepo.GetClientByShortKey(key);
        public async Task<Client> CreateClient(ClientRegistration reg)
        {
            // Check that the short key is not already in use
            var existingClientShortKey = _clientRepo.GetClientByShortKey(reg.ShortKey);
            if (existingClientShortKey != null) throw new Exception("Short Key is already in use");

            var client = new Client
            {
                Id = Guid.NewGuid().ToString(),
                Name = reg.Name,
                SiteUrl = reg.SiteUrl,
                ShortKey = reg.ShortKey,
                ClientApiKey = Guid.NewGuid().ToString()
            };
            return await _clientRepo.CreateClient(client);
        }
        public async Task<Client> DeleteClient(string clientId) => await _clientRepo.DeleteClient(clientId);
        public async Task<Client> UpdateClient<T>(string propertyPath, T data, string clientId) => await _clientRepo.UpdateClient(propertyPath, data, clientId);
        public async Task<Client> UpdateClient(string clientId, Client client)
        {
            return await _clientRepo.UpdateClient(clientId, client);
        }
    }
}
