using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Clients
{
    public interface IClientService
    {
        Task<ClientIdentifier> GetClientIdentifier(string clientIdOrKey);
        Task<List<Client>> GetClients();
        Task<Client> CreateClient(ClientRegistration client);
        Task<Client> DeleteClient(string clientId);
        Task<Client> GetClient(string clientId, bool includeRelatedAuthUsers = false);
        Task<Client> GetClientByShortKey(string key);
        Task<Client> UpdateClient<T>(string propertyPath, T data, string clientId);
        Task<Client> UpdateClient(string clientId, Client client);
        Task<List<PublicClient>> GetClientsByCustomer(string customerEmail);
    }
}