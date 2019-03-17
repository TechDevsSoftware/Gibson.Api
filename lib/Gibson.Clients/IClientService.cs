using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Clients
{
    public interface IClientService
    {
        Task<ClientIdentifier> GetClientIdentifier(string clientIdOrKey);
        Task<List<Client>> GetClients();
        Task<Client> CreateClient(ClientRegistration client);
        Task<Client> DeleteClient(string clientId);
        Task<Client> GetClient(string clientId);
        Task<Client> GetClientByShortKey(string key);
        Task<Client> UpdateClient<T>(string propertyPath, T data, string clientId);
        Task<Client> UpdateClient(string clientId, Client client);
        Task<List<PublicClient>> GetClientsByCustomer(string customerEmail);
    }
}