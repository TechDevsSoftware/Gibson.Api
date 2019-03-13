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

        private bool IsGuid(string key)
        {
            return Guid.TryParse(key, out var guid);
        }

        public async Task<ClientIdentifier> GetClientIdentifier(string clientIdOrKey)
        {
            // If the clientId is not a valid Guid, check for shortkey
            FilterDefinition<Client> filter;
            filter = IsGuid(clientIdOrKey)
                ? Builders<Client>.Filter.Eq(p => p.Id, clientIdOrKey)
                : Builders<Client>.Filter.Eq(p => p.ShortKey, clientIdOrKey);

            var fields = Builders<Client>.Projection.Include(p => p.Id).Include(p => p.ShortKey).Include(p => p.Name);
            var options = new FindOptions<Client>() { Projection = fields };
            var result = await _clients.FindAsync(filter, options);
            var client = await result.FirstOrDefaultAsync();
            return new ClientIdentifier { Id = client.Id, Name = client.Name, ShortKey = client.ShortKey };
        }

        public async Task<List<Client>> GetClients()
        {
            var clients = await _clients.FindAsync(x => true);
            var result = await clients.ToListAsync();
            return result;
        }

        public async Task<Client> GetClient(string clientId)
        {
            var client = await _clients.Find(x => x.Id == clientId).FirstAsync();
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
            return await GetClient(clientId);
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
