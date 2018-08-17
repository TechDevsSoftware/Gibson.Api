using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace TechDevs.Accounts
{
    public interface IAuthUserRepository
    {
        Task<List<IAuthUser>> GetAll();
        Task<IAuthUser> Insert(IAuthUser user);
        Task<IAuthUser> SetEmail(IAuthUser user, string email);
        Task<IAuthUser> SetUsername(IAuthUser user, string username);
        Task<IAuthUser> SetName(IAuthUser user, string firstName, string lastName);
        Task<IAuthUser> SetPassword(IAuthUser user, string hashedPassword);
        Task<IAuthUser> FindByEmail(string email);
        Task<IAuthUser> FindById(string id);
        Task<IAuthUser> FindByProvider(string provider, string providerId);
        Task<bool> Delete(IAuthUser user);
        Task<bool> UserExists(string email);
        Task<IAuthUser> UpdateUser<Type>(string propertyPath, List<Type> data, string id);
    }

    public class MongoUserRepository : IAuthUserRepository
    {
        readonly IMongoDatabase _database;
        readonly IMongoCollection<IAuthUser> _users;
        readonly IStringNormaliser _normaliser;

        public MongoUserRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) _database = client.GetDatabase(dbSettings.Value.Database);
            if (_database != null) _users = _database.GetCollection<IAuthUser>("users");
            _normaliser = normaliser;
        }

        public async Task<bool> Delete(IAuthUser user)
        {
            DeleteResult result = await _users.DeleteOneAsync(FilterByEmail(user.EmailAddress));
            return (result.IsAcknowledged && result.DeletedCount > 0);
        }

        public async Task<IAuthUser> FindByEmail(string email)
        {
            var result = await _users.Find(FilterByEmail(email)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<IAuthUser> FindByProvider(string provider, string providerId)
        {
            var result = await _users.Find(FilterByProvider(provider, providerId)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<IAuthUser>> GetAll()
        {
            var results = await _users.Find(_ => true).ToListAsync();
            return results;
        }

        public async Task<IAuthUser> Insert(IAuthUser user)
        {
            user.NormalisedEmail = _normaliser.Normalise(user.EmailAddress);
            user.Username = user.EmailAddress;
            user.NormalisedUsername = _normaliser.Normalise(user.EmailAddress);

            await _users.InsertOneAsync(user);
            var result = await FindByEmail(user.EmailAddress);
            return result;
        }

        public async Task<IAuthUser> SetEmail(IAuthUser user, string email)
        {
            UpdateDefinition<IAuthUser> update = Builders<IAuthUser>
                .Update
                .Set("EmailAddress", email)
                .Set("NormalisedEmail", _normaliser.Normalise(email));

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindByEmail(email);
            throw new Exception("User email could not be updated");
        }

        public async Task<IAuthUser> SetName(IAuthUser user, string firstName, string lastName)
        {
            UpdateDefinition<IAuthUser> update = Builders<IAuthUser>
               .Update
               .Set("FirstName", firstName)
               .Set("LastName", lastName);

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            return await FindByEmail(user.EmailAddress);
        }

        public async Task<IAuthUser> SetUsername(IAuthUser user, string username)
        {
            UpdateDefinition<IAuthUser> update = Builders<IAuthUser>
              .Update
              .Set("Username", username)
              .Set("NormalisedUsername", _normaliser.Normalise(username));

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            return await FindByEmail(user.EmailAddress);
        }

        public async Task<IAuthUser> SetPassword(IAuthUser user, string passwordHash)
        {
            UpdateDefinition<IAuthUser> update = Builders<IAuthUser>
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

        public async Task<IAuthUser> UpdateUser<Type>(string propertyPath, List<Type> data, string id)
        {
            UpdateDefinition<IAuthUser> update = Builders<IAuthUser>
                .Update
                .Set(propertyPath, data);

            var result = await _users.UpdateOneAsync(FilterById(id), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindById(id);
            throw new Exception("User could not be updated");
        }

        FilterDefinition<IAuthUser> FilterByEmail(string email)
        {
            var normEmail = _normaliser.Normalise(email);
            var filter = Builders<IAuthUser>.Filter.Eq("NormalisedEmail", normEmail);
            return filter;
        }

        FilterDefinition<IAuthUser> FilterByProvider(string provider, string providerId)
        {
            var filter = Builders<IAuthUser>.Filter.Eq("ProviderId", providerId);
            return filter;
        }

        FilterDefinition<IAuthUser> FilterById(string id)
        {
            var filter = Builders<IAuthUser>.Filter.Eq("_id", id);
            return filter;
        }

        public async Task<IAuthUser> FindById(string id)
        {
            var result = await _users.Find(FilterById(id)).FirstOrDefaultAsync();
            return result;
        }

    }
}
