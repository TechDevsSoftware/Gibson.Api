using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechDevs.Accounts.Services
{
    public interface IClientService
    {
        Task<IClient> CreateClient(IClient client);
        Task<IClient> DeleteClient(string clientId);
        Task<IClient> GetClient(string clientId);
        Task<IClient> UpdateClient(string propertyPath, List<Type> data, string clientId);
    }
}