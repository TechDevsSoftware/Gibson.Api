using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TechDevs.Shared.Models;

namespace Gibson.CustomerVehicles
{

    public abstract class ClientDataRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        readonly IMongoDatabase database;
        readonly IMongoCollection<TEntity> collection;

        protected ClientDataRepository(string collectionName, IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) database = client.GetDatabase(dbSettings.Value.Database);
            if (database != null) collection = database.GetCollection<TEntity>(collectionName);
        }

        public virtual async Task<TEntity> Create(TEntity entity, Guid clientId)
        {
            entity.Id = Guid.NewGuid();
            entity.ClientId = clientId;
            await collection.InsertOneAsync(entity);
            return await FindById(entity.Id, clientId);
        }

        public virtual async Task Delete(Guid id, Guid clientId)
        {
            await collection.FindOneAndDeleteAsync(x => x.Id == id && x.ClientId == clientId);
        }

        public virtual async Task<List<TEntity>> FindAll(Guid clientId)
        {
            var results = await collection.FindAsync(x => x.ClientId == clientId);
            return await results.ToListAsync();
        }

        public virtual async Task<TEntity> FindById(Guid id, Guid clientId)
        {
            var result = await collection.FindAsync(x => x.Id == id && x.ClientId == clientId);
            return result.FirstOrDefault();
        }

        public virtual async Task<TEntity> Update(TEntity entity, Guid clientId)
        {
            var filter = new BsonDocument { { "_id", entity.Id }, { "ClientId", clientId } };
            var result = await collection.ReplaceOneAsync(x => x.Id == entity.Id && x.ClientId == clientId, entity, new UpdateOptions { IsUpsert = false });
            return await FindById(entity.Id, clientId);
        }
    }
}
