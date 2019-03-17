using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Clients
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepo;

        public ClientService(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public async Task<ClientIdentifier> GetClientIdentifier(string clientIdOrKey) => await _clientRepo.GetClientIdentifier(clientIdOrKey);
        public async Task<List<Client>> GetClients() => await _clientRepo.GetClients();
        public async Task<Client> GetClient(string clientId) => await _clientRepo.GetClient(clientId);
        public async Task<Client> GetClientByShortKey(string key) => await _clientRepo.GetClientByShortKey(key);
        public async Task<Client> CreateClient(ClientRegistration reg)
        {
            // Check that the short key is not already in use
            if (!await ShortKeyAvailable(reg.ShortKey)) throw new Exception("Short Key is already in use");

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
            // Check to see if the short key has changed
            var existingClient = await _clientRepo.GetClient(clientId);
            if (existingClient == null) throw new Exception("Existing client not found");
            if (existingClient.ShortKey != client.ShortKey)
            {
                if (!await ShortKeyAvailable(client.ShortKey)) throw new Exception("Short Key is already in use");
            }
            return await _clientRepo.UpdateClient(clientId, client);
        }
        public async Task<List<PublicClient>> GetClientsByCustomer(string customerEmail)
        {
            var clients = await _clientRepo.GetClientsByCustomer(customerEmail);
            var result = clients.Select(c => new PublicClient(c));
            return result.ToList();
        }
        private async Task<bool> ShortKeyAvailable(string clientKey)
        {
            var existingClientShortKey = await _clientRepo.GetClientByShortKey(clientKey);
            return existingClientShortKey == null;
        }
    }
}
