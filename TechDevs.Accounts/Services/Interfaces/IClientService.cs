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
        Task<Client> UpdateClient(string propertyPath, List<Type> data, string clientId);
    }
}