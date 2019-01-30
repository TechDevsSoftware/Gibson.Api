using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TechDevs.Shared.Models;
using TechDevs.Shared.Models.Shared;

namespace TechDevs.Clients.BookingRequests
{
    public abstract class MongoBaseRepository<TEntity> : IRepository<TEntity> where TEntity : ClientEntity
    {
        readonly IMongoDatabase database;
        readonly IMongoCollection<TEntity> collection;
        private readonly IClientService clientService;

        protected MongoBaseRepository(string collectionName, IOptions<MongoDbSettings> dbSettings, IClientService clientService)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) database = client.GetDatabase(dbSettings.Value.Database);
            if (database != null) collection = database.GetCollection<TEntity>(collectionName);
            this.clientService = clientService;
        }

        public async Task<TEntity> Create(TEntity entity, string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            entity.ClientId = client.Id;
            await collection.InsertOneAsync(entity);
            return await FindById(entity.Id, client.Id);
        }

        public async Task Delete(string id, string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            await collection.FindOneAndDeleteAsync(x => x.Id == id && x.ClientId == client.Id);
        }

        public async Task<List<TEntity>> FindAll(string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            var results = await collection.FindAsync(x => x.ClientId == client.Id);
            return await results.ToListAsync();
        }

        public async Task<TEntity> FindById(string id, string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            var result = await collection.FindAsync(x => x.Id == id && x.ClientId == client.Id);
            return result.FirstOrDefault();
        }

        public async Task<TEntity> Update(TEntity entity, string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            var filter = new BsonDocument { { "_id", entity.Id }, { "ClientId", client.Id } };
            var result = await collection.ReplaceOneAsync(x => x.Id == entity.Id && x.ClientId == client.Id, entity, new UpdateOptions { IsUpsert = false });
            return await FindById(entity.Id, client.Id);
        }
    }
}
