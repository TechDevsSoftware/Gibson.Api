using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Clients
{
    public interface IClientRepository
    {
        Task<ClientIdentifier> GetClientIdentifier(string clientIdOrKey);
        Task<List<Client>> GetClients();
        Task<Client> GetClient(string clientId);
        Task<Client> CreateClient(Client client);
        Task<Client> UpdateClient<Type>(string propertyPath, Type data, string clientId);
        Task<Client> UpdateClient(string clientId, Client client);
        Task<Client> DeleteClient(string clientId);
        Task<Client> GetClientByShortKey(string shortKey);
        Task<List<Client>> GetClientsByCustomer(string customerEmail);
    }
}
