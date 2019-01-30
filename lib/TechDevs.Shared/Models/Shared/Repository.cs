using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechDevs.Shared.Models.Shared
{

    public class ClientEntity
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
    }

    public interface IRepository<T> where T : ClientEntity
    {
        Task<List<T>> FindAll(string clientId);
        Task<T> FindById(string id, string clientId);
        Task<T> Create(T entity, string clientId);
        Task<T> Update(T entity, string clientId);
        Task Delete(string id, string clientId);
    }

}
