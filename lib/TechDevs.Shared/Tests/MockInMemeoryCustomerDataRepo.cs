using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.Shared.Repositories.Tests
{
    public class MockInMemeoryCustomerDataRepo<T> where T : CustomerEntity
    {
        private List<T> collection = new List<T>();

        public Task<T> Create(T entity, Guid customerId, Guid clientId)
        {
            collection.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<List<T>> FindAllAnyCustomer(Guid clientId)
        {
            return Task.FromResult(collection.Where(x => x.ClientId == clientId).ToList());
        }

        public Task<List<T>> FindAll(Guid customerId, Guid clientId)
        {
            var results = collection.Where(x => x.CustomerId == customerId && x.ClientId == clientId).ToList();
            return Task.FromResult(results);
        }

        public Task<T> FindById(Guid id, Guid customerId, Guid clientId)
        {
            var result = collection.FirstOrDefault(x => x.Id == id && x.ClientId == clientId && x.CustomerId == customerId);
            return Task.FromResult(result);
        }

        public Task<T> Update(T entity, Guid customerId, Guid clientId)
        {
            var index = collection.FindIndex(x => x.Id == entity.Id && x.ClientId == clientId && x.CustomerId == customerId);
            if (index == -1) throw new Exception("Cannot be updated as no matching record found");
            collection[index] = entity;
            return Task.FromResult(entity);
        }

        public Task Delete(Guid id, Guid customerId, Guid clientId)
        {
            var index = collection.FindIndex(x => x.Id == id && x.ClientId == clientId && x.CustomerId == customerId);
            collection.RemoveAt(index);
            return Task.CompletedTask;
        }

        public void Reset()
        {
            collection = new List<T>();
        }

        public int RowCount()
        {
            return collection.Count;
        }
    }
}
