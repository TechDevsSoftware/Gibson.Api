using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechDevs.Shared.Models.Shared
{
    public class Repository
    {
        public interface IClientEntity
        {
            Guid Id { get; set; }
            Guid ClientId { get; set; }
        }

        public interface IRepository<T> where T : class, IClientEntity
        {
            Task<List<T>> FindAll(Guid clientId);
            Task<T> FindById(Guid id, Guid clientId);
            Task<T> Create(T entity, Guid clientId);
            Task<T> Update(T entity, Guid clientId);
            Task Delete(Guid id, Guid clientId);
        }
    }
}
