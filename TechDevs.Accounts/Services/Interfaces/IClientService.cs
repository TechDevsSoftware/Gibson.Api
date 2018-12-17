using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechDevs.Accounts.Services
{
    public interface IClientService
    {
        Task<List<Client>> GetClients();
        Task<Client> CreateClient(ClientRegistration client);
        Task<Client> DeleteClient(string clientId);
        Task<Client> GetClient(string clientId, bool includeRelatedAuthUsers = false);
        Task<Client> GetClientByShortKey(string key);
        Task<Client> UpdateClient<T>(string propertyPath, T data, string clientId);
        Task<Client> UpdateClient(string clientId, Client client);
    }
}