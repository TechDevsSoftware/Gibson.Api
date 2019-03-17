using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Gibson.Common.Models;

namespace Gibson.Shared.Repositories
{
    public class ClientDataRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly IMongoCollection<TEntity> collection;

        public ClientDataRepository(string collectionName, IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            var database = client.GetDatabase(dbSettings.Value.Database);
            if (database != null) collection = database.GetCollection<TEntity>(collectionName);
        }

        public virtual async Task<TEntity> Create(TEntity entity, Guid clientId)
        {
            if (clientId == Guid.Empty) throw new Exception("Client Id cannot be an empty GUID");

            entity.Id = Guid.NewGuid();
            entity.ClientId = clientId;
            await collection.InsertOneAsync(entity);
            return await FindById(entity.Id, clientId);
        }

        public virtual async Task Delete(Guid id, Guid clientId)
        {
            if (clientId == Guid.Empty) throw new Exception("Client Id cannot be an empty GUID");
            await collection.FindOneAndDeleteAsync(x => x.Id == id && x.ClientId == clientId);
        }

        public virtual async Task<List<TEntity>> FindAllByCustomer(Guid clientId)
        {
            if (clientId == Guid.Empty) throw new Exception("Client Id cannot be an empty GUID");

            var results = await collection.FindAsync(x => x.ClientId == clientId);
            return await results.ToListAsync();
        }

        public virtual async Task<List<TEntity>> FindAll(Guid clientId)
        {
            if (clientId == Guid.Empty) throw new Exception("Client Id cannot be an empty GUID");
            var results = await collection.FindAsync(x => x.ClientId == clientId);
            return await results.ToListAsync();
        }

        public virtual async Task<TEntity> FindById(Guid id, Guid clientId)
        {
            if (clientId == Guid.Empty) throw new Exception("Client Id cannot be an empty GUID");
            var result = await collection.Find(x => x.Id == id && x.ClientId == clientId).FirstOrDefaultAsync();
            return result;
        }

        public virtual async Task<TEntity> Update(TEntity entity, Guid clientId)
        {
            if (clientId == Guid.Empty) throw new Exception("Client Id cannot be an empty GUID");
            var result = await collection.ReplaceOneAsync(x =>
                x.Id == entity.Id && x.ClientId == clientId
                , entity, new UpdateOptions { IsUpsert = false });
            return await FindById(entity.Id, clientId);
        }
    }
}
