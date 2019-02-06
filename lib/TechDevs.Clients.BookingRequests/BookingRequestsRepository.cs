using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace Gibson.BookingRequests
{
    public class BookingRequestsRepository : IBookingRequestsRepository
    {
        readonly IMongoDatabase database;
        readonly IMongoCollection<BookingRequest> collection;
        private readonly IClientService clientService;
        
        public BookingRequestsRepository(IOptions<MongoDbSettings> dbSettings, IClientService clientService) 
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) database = client.GetDatabase(dbSettings.Value.Database);
            if (database != null) collection = database.GetCollection<BookingRequest>("BookingRequests");
            this.clientService = clientService;
        }

        public async Task<BookingRequest> Create(BookingRequest entity, string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            entity.ClientId = client.Id;
            await collection.InsertOneAsync(entity);
            return await FindById(entity.Id, client.Id);
        }

        public async Task Delete(string id, string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            var filter = new BsonDocument { { "_id", id }, { "ClientId", client.Id } };
            await collection.FindOneAndDeleteAsync(filter);
        }

        public async Task<List<BookingRequest>> FindAll(string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            var results = await collection.FindAsync(Builders<BookingRequest>.Filter.Eq("ClientId", client.Id));
            return await results.ToListAsync();
        }

        public async Task<BookingRequest> FindById(string id, string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            var filter = new BsonDocument { { "_id", id }, { "ClientId", client.Id } };
            var result = await collection.FindAsync(filter);
            return result.FirstOrDefault();
        }

        public async Task<BookingRequest> Update(BookingRequest entity, string clientIdOrKey)
        {
            var client = await clientService.GetClientIdentifier(clientIdOrKey);
            var filter = new BsonDocument { { "_id", entity.Id }, { "ClientId", client.Id } };
            var result = await collection.ReplaceOneAsync(x => x.Id == entity.Id && x.ClientId == client.Id, entity, new UpdateOptions { IsUpsert = false });
            return await FindById(entity.Id, client.Id);
        }
    }
}
