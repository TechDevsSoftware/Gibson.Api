using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace TechDevs.Accounts
{
    public interface IUserRepository
    {
        Task<List<IUser>> GetAll();
        Task<IUser> Insert(IUser user);
        Task<IUser> SetEmail(IUser user, string email);
        Task<IUser> SetUsername(IUser user, string username);
        Task<IUser> SetName(IUser user, string firstName, string lastName);
		Task<IUser> SetPassword(IUser user, string hashedPassword);
        Task<IUser> FindByEmail(string email);
        Task<bool> Delete(IUser user);
        Task<bool> UserExists(string email);
    }

    public class MongoUserRepository : IUserRepository
    {
        readonly IMongoDatabase _database;
        readonly IMongoCollection<IUser> _users;
        readonly IStringNormaliser _normaliser;

        public MongoUserRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) _database = client.GetDatabase(dbSettings.Value.Database);
            if (_database != null) _users = _database.GetCollection<IUser>("users");
            _normaliser = normaliser;
        }

        public async Task<bool> Delete(IUser user)
        {
            DeleteResult result = await _users.DeleteOneAsync(FilterByEmail(user.EmailAddress));
            return (result.IsAcknowledged && result.DeletedCount > 0);
        }

        public async Task<IUser> FindByEmail(string email)
        {
            var result = await _users.Find(FilterByEmail(email)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<IUser>> GetAll()
        {
            var results = await _users.Find(_ => true).ToListAsync();
            return results;
        }

        public async Task<IUser> Insert(IUser user)
        {
            user.NormalisedEmail = _normaliser.Normalise(user.EmailAddress);
            user.Username = user.EmailAddress;
            user.NormalisedUsername = _normaliser.Normalise(user.EmailAddress);

            await _users.InsertOneAsync(user);
            var result = await FindByEmail(user.EmailAddress);
            return result;
        }

        public async Task<IUser> SetEmail(IUser user, string email)
        {
            UpdateDefinition<IUser> update = Builders<IUser>
                .Update
                .Set("EmailAddress", email)
                .Set("NormalisedEmail", _normaliser.Normalise(email));

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindByEmail(email);
            throw new Exception("User email could not be updated");
        }

        public async Task<IUser> SetName(IUser user, string firstName, string lastName)
        {
            UpdateDefinition<IUser> update = Builders<IUser>
               .Update
               .Set("FirstName", firstName)
               .Set("LastName", lastName);

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            return await FindByEmail(user.EmailAddress);
        }

        public async Task<IUser> SetUsername(IUser user, string username)
        {
            UpdateDefinition<IUser> update = Builders<IUser>
              .Update
              .Set("Username", username)
              .Set("NormalisedUsername", _normaliser.Normalise(username));
            
            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress), update);
            return await FindByEmail(user.EmailAddress);
        }

        public async Task<IUser> SetPassword(IUser user, string passwordHash)
        {
            UpdateDefinition<IUser> update = Builders<IUser>
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

        FilterDefinition<IUser> FilterByEmail(string email)
        {
            var normEmail = _normaliser.Normalise(email);
            var filter = Builders<IUser>.Filter.Eq("NormalisedEmail", normEmail);
            return filter;
        }
    }
}
     