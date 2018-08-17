using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TechDevs.Accounts.Repositories
{
    public interface IClientRepository
    {
        Task<IClient> GetClient(string clientId);
        Task<IClient> CreateClient(IClient client);
        Task<IClient> UpdateClient<Type>(string propertyPath, List<Type> data, string clientId);
        Task<IClient> DeleteClient(string clientId);
    }

    public class ClientRepository : IClientRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<IAuthUser> _users;
        private readonly IMongoCollection<IClient> _clients;

        public ClientRepository(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            _database = client.GetDatabase(dbSettings.Value.Database);
            _clients = _database.GetCollection<IClient>("Clients");
        }
        
        public async Task<IClient> GetClient(string clientId)
        {
            var filter = Builders<IClient>.Filter.Eq(x => x.Id, clientId);
            var client = _clients.Find(x => x.Id == clientId).FirstOrDefaultAsync();
            return await client;
        }

        public async Task<IClient> CreateClient(IClient client)
        {
            await _clients.InsertOneAsync(client);
            var result = await GetClient(client.Id);
            return result;
        }

        public async Task<IClient> DeleteClient(string clientId)
        {
            var result = await _clients.FindOneAndDeleteAsync(x => x.Id == clientId);
            return result;
        }

        public async Task<IClient> UpdateClient<Type>(string propertyPath, List<Type> data, string clientId)
        {
            FilterDefinition<IClient> filter = Builders<IClient>.Filter.Eq(x => x.Id, clientId);
            UpdateDefinition<IClient> update = Builders<IClient>.Update.Set(propertyPath, data);

            var result = await _clients.UpdateOneAsync(filter, update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await GetClient(clientId);
            throw new Exception("User could not be updated");
        }
    }
}
