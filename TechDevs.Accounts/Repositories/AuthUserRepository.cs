using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TechDevs.Accounts.Repositories
{
    public class CustomerRepository : AuthUserRepository<Customer>
    {
        public CustomerRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser) : base(dbSettings, normaliser)
        {
        }
    }

    public class EmployeeRepository : AuthUserRepository<Employee>
    {
        public EmployeeRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser) : base(dbSettings, normaliser)
        {
        }
    }

    public abstract class AuthUserRepository<TAuthUser> : IAuthUserRepository<TAuthUser> where TAuthUser : IAuthUser
    {
        readonly IMongoDatabase _database;
        readonly IMongoCollection<TAuthUser> __users;
        readonly IStringNormaliser _normaliser;

        public AuthUserRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) _database = client.GetDatabase(dbSettings.Value.Database);
            if (_database != null) __users = _database.GetCollection<TAuthUser>("AuthUsers");
            _normaliser = normaliser;
        }

        public async Task<TAuthUser> Insert(TAuthUser user, string clientId)
        {
            user.NormalisedEmail = _normaliser.Normalise(user.EmailAddress);
            user.Username = user.EmailAddress;
            user.NormalisedUsername = _normaliser.Normalise(user.EmailAddress);

            await __users.InsertOneAsync(user);
            var result = await FindByEmail(user.EmailAddress, clientId);
            return result;
        }

        public async Task<bool> Delete(TAuthUser user, string clientId)
        {
            DeleteResult result = await __users.DeleteOneAsync(FilterByEmail(user.EmailAddress, clientId));
            return (result.IsAcknowledged && result.DeletedCount > 0);
        }

        public async Task<TAuthUser> FindById(string id, string clientId)
        {
            var result = await __users.Find(FilterById(id, clientId)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<TAuthUser> FindByEmail(string email, string clientId)
        {
            var result = await __users.Find(FilterByEmail(email, clientId)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<TAuthUser> FindByProvider(string provider, string providerId, string clientId)
        {
            var result = await __users.Find(FilterByProvider(provider, providerId)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<TAuthUser>> FindAll(string clientId)
        {
            var results = await __users.Find(_ => true).ToListAsync();
            return results;
        }

        public async Task<TAuthUser> SetEmail(TAuthUser user, string email, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
                .Update
                .Set("EmailAddress", email)
                .Set("NormalisedEmail", _normaliser.Normalise(email));

            var result = await __users.UpdateOneAsync(FilterByEmail(user.EmailAddress, clientId), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindByEmail(email, clientId);
            throw new Exception("User email could not be updated");
        }

        public async Task<TAuthUser> SetName(TAuthUser user, string firstName, string lastName, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
               .Update
               .Set("FirstName", firstName)
               .Set("LastName", lastName);

            var result = await __users.UpdateOneAsync(FilterByEmail(user.EmailAddress, clientId), update);
            return await FindByEmail(user.EmailAddress, clientId);
        }

        public async Task<TAuthUser> SetUsername(TAuthUser user, string username, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
              .Update
              .Set("Username", username)
              .Set("NormalisedUsername", _normaliser.Normalise(username));

            var result = await __users.UpdateOneAsync(FilterByEmail(user.EmailAddress, clientId), update);
            return await FindByEmail(user.EmailAddress, clientId);
        }

        public async Task<TAuthUser> SetPassword(TAuthUser user, string passwordHash, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
                .Update
                .Set("PasswordHash", passwordHash);

            var result = await __users.UpdateOneAsync(FilterByEmail(user.EmailAddress, clientId), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindByEmail(user.EmailAddress, clientId);
            throw new Exception("Password could not be updated");
        }

        public async Task<bool> UserExists(string email, string clientId)
        {
            var results = await FindByEmail(email, clientId);
            return (results != null);
        }

        public async Task<TAuthUser> UpdateUser<Type>(string propertyPath, List<Type> data, string id, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
                .Update
                .Set(propertyPath, data);

            var result = await __users.UpdateOneAsync(FilterById(id, clientId), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindById(id, clientId);
            throw new Exception("User could not be updated");
        }
        
        private FilterDefinition<TAuthUser> FilterByProvider(string provider, string providerId) => Builders<TAuthUser>.Filter.And(
            Builders<TAuthUser>.Filter.Eq(x => x.ProviderId, providerId),
            Builders<TAuthUser>.Filter.Eq(x => x.ProviderName, provider)
        );

        private FilterDefinition<TAuthUser> FilterById(string id, string clientId) => Builders<TAuthUser>.Filter.And(
            Builders<TAuthUser>.Filter.Eq(x => x.Id, id),
            Builders<TAuthUser>.Filter.Eq(x => x.ClientId.Id, clientId)
        );

        private FilterDefinition<TAuthUser> FilterByEmail(string email, string clientId) => Builders<TAuthUser>.Filter.And(
            Builders<TAuthUser>.Filter.Eq(x => x.NormalisedEmail, email),
            Builders<TAuthUser>.Filter.Eq(x => x.ClientId.Id, clientId)
        );

    }
}
