using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace TechDevs.Accounts
{
    public interface AuthUserRepository
    {
        Task<List<AuthUser>> GetAll();
        Task<AuthUser> Insert(AuthUser user);
        Task<AuthUser> SetEmail(AuthUser user, string email);
        Task<AuthUser> SetUsername(AuthUser user, string username);
        Task<AuthUser> SetName(AuthUser user, string firstName, string lastName);
        Task<AuthUser> SetPassword(AuthUser user, string hashedPassword);
        Task<AuthUser> FindByEmail(string email);
        Task<AuthUser> FindById(string id);
        Task<AuthUser> FindByProvider(string provider, string providerId);
        Task<bool> Delete(AuthUser user);
        Task<bool> UserExists(string email);
        Task<AuthUser> UpdateUser<Type>(string propertyPath, List<Type> data, string id);
    }

    public class MongoUserRepository : AuthUserRepository
    {
        readonly IMongoDatabase _database;
        readonly IMongoCollection<AuthUser> _users;
        readonly IStringNormaliser _normaliser;

        public MongoUserRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) _database = client.GetDatabase(dbSettings.Value.Database);
            if (_database != null) _users = _database.GetCollection<AuthUser>("users");
            _normaliser = normaliser;
        }

        public async Task<bool> Delete(AuthUser user)
        {
            DeleteResult result = await _users.DeleteOneAsync(FilterByEmail(user.EmailAddress));
            return (result.IsAcknowledged && result.DeletedCount > 0);
        }

        public async Task<AuthUser> FindByEmail(string email)
        {
            var result = await _users.Find(FilterByEmail(email)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<AuthUser> FindByProvider(string provider, string providerId)
        {
            var result = await _users.Find(FilterByProvider(provider, providerId)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<AuthUser>> GetAll()
        {
            var results = await _users.Find(_ => true).ToListAsync();
            return results;
        }

        public async Task<AuthUser> Insert(AuthUser user)
        {
            user.NormalisedEmail = _normaliser.Normalise(user.EmailAddress);
            user.Username = user.EmailAddress;
            user.NormalisedUsername = _normaliser.Normalise(user.EmailAddress);

            await _users.InsertOneAsync(user);
            var result = await FindByEmail(user.EmailAddress);
            return result;
        }

        public async Task<AuthUser> SetEmail(AuthUser user, string email)
        {
            UpdateDefinition<AuthUser> update = Builders<AuthUser>
                .Update
                .Set("EmailAddress", email)
                .Set("NormalisedEmail", _normaliser.Normalise(email));

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindByEmail(email);
            throw new Exception("User email could not be updated");
        }

        public async Task<AuthUser> SetName(AuthUser user, string firstName, string lastName)
        {
            UpdateDefinition<AuthUser> update = Builders<AuthUser>
               .Update
               .Set("FirstName", firstName)
               .Set("LastName", lastName);

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            return await FindByEmail(user.EmailAddress);
        }

        public async Task<AuthUser> SetUsername(AuthUser user, string username)
        {
            UpdateDefinition<AuthUser> update = Builders<AuthUser>
              .Update
              .Set("Username", username)
              .Set("NormalisedUsername", _normaliser.Normalise(username));

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            return await FindByEmail(user.EmailAddress);
        }

        public async Task<AuthUser> SetPassword(AuthUser user, string passwordHash)
        {
            UpdateDefinition<AuthUser> update = Builders<AuthUser>
                .Update
                .Set("PasswordHash", passwordHash);

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindByEmail(user.EmailAddress);
            throw new Exception("Password could not be updated");
        }

        public async Task<bool> UserExists(string email)
        {
            var results = await FindByEmail(email);
            return (results != null);
        }

        public async Task<AuthUser> UpdateUser<Type>(string propertyPath, List<Type> data, string id)
        {
            UpdateDefinition<AuthUser> update = Builders<AuthUser>
                .Update
                .Set(propertyPath, data);

            var result = await _users.UpdateOneAsync(FilterById(id), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindById(id);
            throw new Exception("User could not be updated");
        }

        FilterDefinition<AuthUser> FilterByEmail(string email)
        {
            var normEmail = _normaliser.Normalise(email);
            var filter = Builders<AuthUser>.Filter.Eq("NormalisedEmail", normEmail);
            return filter;
        }

        FilterDefinition<AuthUser> FilterByProvider(string provider, string providerId)
        {
            var filter = Builders<AuthUser>.Filter.Eq("ProviderId", providerId);
            return filter;
        }

        FilterDefinition<AuthUser> FilterById(string id)
        {
            var filter = Builders<AuthUser>.Filter.Eq("_id", id);
            return filter;
        }

        public async Task<AuthUser> FindById(string id)
        {
            var result = await _users.Find(FilterById(id)).FirstOrDefaultAsync();
            return result;
        }

    }
}
