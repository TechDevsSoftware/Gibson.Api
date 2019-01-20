using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;

namespace TechDevs.Clients
{
    public class ClientRepository : IClientRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<AuthUser> _users;
        private readonly IMongoCollection<Customer> _customers;
        private readonly IMongoCollection<Client> _clients;

        public ClientRepository(IOptions<MongoDbSettings> dbSettings)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            _database = client.GetDatabase(dbSettings.Value.Database);
            _clients = _database.GetCollection<Client>("Clients");
            _users = _database.GetCollection<AuthUser>("AuthUsers");
            _customers = _database.GetCollection<Customer>("AuthUsers");
        }

        public async Task<List<Client>> GetClients()
        {
            var clients = await _clients.FindAsync(x => true);
            var result = await clients.ToListAsync();
            return result;
        }

        public async Task<Client> GetClient(string clientId, bool includeRelatedAuthUsers = false)
        {
            var client = await _clients.Find(x => x.Id == clientId).FirstAsync();
            if (includeRelatedAuthUsers)
            {
                var clientFilter = new BsonDocument { { "ClientId", new BsonDocument { { "_id", client.Id } } } };

                var employees = await _users.OfType<Employee>().Find(clientFilter).ToListAsync();
                var customers = await _users.OfType<Customer>().Find(clientFilter).ToListAsync();

                client.Employees = employees.Select(e => new AuthUserProfile(e)).ToList();
                client.Customers = customers.Select(e => new AuthUserProfile(e)).ToList();
            }
            return client;
        }

        public async Task<Client> CreateClient(Client client)
        {
            await _clients.InsertOneAsync(client);
            return await GetClient(client.Id);
        }

        public async Task<Client> DeleteClient(string clientId)
        {
            return await _clients.FindOneAndDeleteAsync(x => x.Id == clientId);
        }

        public async Task<Client> UpdateClient<Type>(string propertyPath, Type data, string clientId)
        {
            propertyPath = propertyPath.FirstLetterUpper();

            FilterDefinition<Client> filter = Builders<Client>.Filter.Eq(x => x.Id, clientId);
            UpdateDefinition<Client> update = Builders<Client>.Update.Set(propertyPath, data);

            var result = await _clients.UpdateOneAsync(filter, update);
            if (result.IsAcknowledged) return await GetClient(clientId);
            throw new Exception("Client could not be updated");
        }

        public async Task<Client> UpdateClient(string clientId, Client client)
        {
            FilterDefinition<Client> filter = Builders<Client>.Filter.Eq(x => x.Id, clientId);
            var updateOptions = new UpdateOptions { IsUpsert = false };

            client.Id = clientId;

            var result = await _clients.ReplaceOneAsync(filter, client, updateOptions);
            if (!result.IsAcknowledged) throw new Exception("Client could not be updated");
            return await GetClient(clientId, false);
        }

        public async Task<Client> GetClientByShortKey(string shortKey)
        {
            var client = await _clients.Find(x => x.ShortKey == shortKey).FirstOrDefaultAsync();
            return client;
        }

        public async Task<List<Client>> GetClientsByCustomer(string customerEmail)
        {
            var customers = await _customers.Find(x => x.EmailAddress == customerEmail).ToListAsync();
            var clientIds = customers.Select(x => x.ClientId.Id);
            var clients = await _clients.Find(x => clientIds.Contains(x.Id)).ToListAsync();
            return clients;
        }
    }
}
