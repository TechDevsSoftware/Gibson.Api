using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechDevs.Shared.Models;

namespace Gibson.CustomerVehicles
{
    public abstract class CustomerDataRepository<TEntity> : ICustomerDataRepository<TEntity> where TEntity : CustomerEntity
    {
        readonly IMongoDatabase database;
        readonly IMongoCollection<TEntity> collection;

        protected CustomerDataRepository(string collectionName, IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) database = client.GetDatabase(dbSettings.Value.Database);
            if (database != null) collection = database.GetCollection<TEntity>(collectionName);
        }

        public virtual async Task<TEntity> Create(TEntity entity, Guid customerId, Guid clientId)
        {
            entity.Id = Guid.NewGuid();
            entity.ClientId = clientId;
            await collection.InsertOneAsync(entity);
            return await FindById(entity.Id, customerId, clientId);
        }

        public virtual async Task Delete(Guid id, Guid customerId, Guid clientId)
        {
            await collection.FindOneAndDeleteAsync(x => x.Id == id && x.ClientId == clientId && x.CustomerId == customerId);
        }

        public virtual async Task<List<TEntity>> FindAll(Guid customerId, Guid clientId)
        {
            var results = await collection.FindAsync(x => x.ClientId == clientId && x.CustomerId == customerId);
            return await results.ToListAsync();
        }

        public virtual async Task<TEntity> FindById(Guid id, Guid customerId, Guid clientId)
        {
            var result = await collection.FindAsync(x => x.Id == id && x.ClientId == clientId && x.CustomerId == customerId);
            return result.FirstOrDefault();
        }

        public virtual async Task<TEntity> Update(TEntity entity, Guid customerId, Guid clientId)
        {
            var result = await collection.ReplaceOneAsync(x =>
                x.Id == entity.Id && x.ClientId == clientId && x.CustomerId == customerId
                , entity, new UpdateOptions { IsUpsert = false });
            return await FindById(entity.Id, customerId, clientId);
        }
    }
}
