using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TechDevs.Shared.Models
{
    public interface IEntity
    {
        Guid Id { get; set; }
        Guid ClientId { get; set; }
    }

    public interface ICustomerEntity : IEntity
    {
        Guid CustomerId { get; set; }
    }

    public abstract class Entity : IEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }

        public Entity()
        {
            Id = Guid.NewGuid();
        }
    }

    public abstract class CustomerEntity : Entity, ICustomerEntity  
    {
        public Guid CustomerId { get; set; }
    }

    public interface IRepository<T> where T : Entity
    {
        Task<List<T>> FindAll(Guid clientId);
        Task<T> FindById(Guid id, Guid clientId);
        Task<T> Create(T entity, Guid clientId);
        Task<T> Update(T entity, Guid clientId);
        Task Delete(Guid id, Guid clientId);
    }

    public interface ICustomerDataRepository<T> where T : ICustomerEntity
    {
        Task<List<T>> FindAll(Guid customerId, Guid clientId);
        Task<List<T>> FindAllAnyCustomer(Guid clientId);
        Task<T> FindById(Guid id, Guid customerId, Guid clientId);
        Task<T> Create(T entity, Guid customerId, Guid clientId);
        Task<T> Update(T entity, Guid customerId, Guid clientId);
        Task Delete(Guid id, Guid customerId, Guid clientId);
    }
}
