using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;
using static TechDevs.Shared.Models.Shared.Repository;

namespace TechDevs.Users
{


    public class MockRepository<T> : IRepository<T> where T : class, IClientEntity
    {
        private List<T> db = new List<T>();

        public Task<T> Create(T entity, Guid clientId)
        {
            db.Add(entity);
            return Task.FromResult(entity);
        }

        public Task Delete(Guid id, Guid clientId)
        {
            db.RemoveAt(db.FindIndex(x => x.Id == id && x.ClientId == clientId));
            return Task.CompletedTask;
        }

        public Task<List<T>> FindAll(Guid clientId)
        {
            var res =  db.Where(x => x.ClientId == clientId).ToList();
            return Task.FromResult(res);
        }

        public Task<T> FindById(Guid id, Guid clientId)
        {
            return Task.FromResult(db.FirstOrDefault(x => x.Id == id && x.ClientId == clientId));
        }

        public Task<T> Update(T entity, Guid clientId)
        {
            var index = db.FindIndex(x => x.Id == entity.Id && x.ClientId == clientId);
            db[index] = entity;
            return Task.FromResult(entity);
        }
    }


    public abstract class AuthUserBaseRepository<TAuthUser> : IAuthUserRepository<TAuthUser> where TAuthUser : AuthUser
    {
        readonly IMongoDatabase _database;
        readonly IMongoCollection<TAuthUser> _users;
        readonly IStringNormaliser _normaliser;

        public AuthUserBaseRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser)
        {
            var client = new MongoClient(dbSettings.Value.ConnectionString);
            if (client != null) _database = client.GetDatabase(dbSettings.Value.Database);
            if (_database != null) _users = _database.GetCollection<TAuthUser>("AuthUsers");
            _normaliser = normaliser;
        }

        public async Task<TAuthUser> Insert(TAuthUser user, string clientId)
        {
            user.NormalisedEmail = _normaliser.Normalise(user.EmailAddress);
            user.Username = user.EmailAddress;
            user.NormalisedUsername = _normaliser.Normalise(user.EmailAddress);
            try
            {
                await _users.InsertOneAsync(user);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            var result = await FindByEmail(user.EmailAddress, clientId);
            return result;
        }

        public async Task<bool> Delete(TAuthUser user, string clientId)
        {
            DeleteResult result = await _users.DeleteOneAsync(FilterByEmail(user.EmailAddress, clientId));
            return (result.IsAcknowledged && result.DeletedCount > 0);
        }

        public virtual async Task<TAuthUser> FindById(string id, string clientId)
        {
            var json = FilterById(id, clientId).ToJson();
            var results = await _users.OfType<TAuthUser>().FindAsync(FilterById(id, clientId));
            var result = await results.FirstOrDefaultAsync();
            return result;
        }

        public virtual async Task<TAuthUser> FindByEmail(string email, string clientId)
        {
            var json = FilterByEmail(email, clientId);
            var results = await _users.OfType<TAuthUser>().FindAsync(FilterByEmail(email, clientId));
            var result = await results.FirstOrDefaultAsync();
            return result;
        }

        public async Task<TAuthUser> FindByProvider(string provider, string providerId, string clientId)
        {
            var filter = new BsonDocument
            {
                { "ClientId", new BsonDocument { { "_id", clientId }}},
                { "ProviderName", provider},
                { "ProviderId", providerId},
            };

            var result = await _users.OfType<TAuthUser>().Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<TAuthUser>> FindAll(string clientId)
        {
            var filter = new BsonDocument { { "ClientId", new BsonDocument { { "_id", clientId } } } };
            var results = await _users.OfType<TAuthUser>().Find(filter).ToListAsync();
            return results;
        }

        public async Task<TAuthUser> SetEmail(TAuthUser user, string email, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
                .Update
                .Set("EmailAddress", email)
                .Set("NormalisedEmail", _normaliser.Normalise(email));

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress, clientId), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindByEmail(email, clientId);
            throw new Exception("User email could not be updated");
        }

        public async Task<TAuthUser> SetName(TAuthUser user, string firstName, string lastName, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
               .Update
               .Set("FirstName", firstName)
               .Set("LastName", lastName);

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress, clientId), update);
            if (!result.IsAcknowledged) throw new Exception("Update failed");
            if (result.ModifiedCount == 0) throw new Exception("Update failed");
            return await FindByEmail(user.EmailAddress, clientId);
        }

        public async Task<TAuthUser> SetUsername(TAuthUser user, string username, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
              .Update
              .Set("Username", username)
              .Set("NormalisedUsername", _normaliser.Normalise(username));

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress, clientId), update);
            return await FindByEmail(user.EmailAddress, clientId);
        }

        public async Task<TAuthUser> SetPassword(TAuthUser user, string passwordHash, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
                .Update
                .Set("PasswordHash", passwordHash);

            var result = await _users.UpdateOneAsync(FilterByEmail(user.EmailAddress, clientId), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindByEmail(user.EmailAddress, clientId);
            throw new Exception("Password could not be updated");
        }

        public async Task<TAuthUser> SetDisabled(string userId, bool disabled, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
           .Update
           .Set("Disabled", disabled);

            var result = await _users.UpdateOneAsync(FilterById(userId, clientId), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindById(userId, clientId);
            throw new Exception("Disabled flag could not be updated");
        }

        public async Task<TAuthUser> SetValidatedEmail(string userId, bool validated, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
           .Update
           .Set("ValidatedEmail", validated);

            var result = await _users.UpdateOneAsync(FilterById(userId, clientId), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindById(userId, clientId);
            throw new Exception("ValidatedEmail flag could not be updated");
        }


        public async Task<bool> UserExists(string email, string clientId)
        {
            var results = await FindByEmail(email, clientId);
            return (results != null);
        }

        public async Task<TAuthUser> UpdateUser<Type>(string propertyPath, List<Type> data, string id, string clientId)
        {
            var filter = new BsonDocument
            {
                { "ClientId", new BsonDocument { { "_id", clientId }}},
                { "_id", id}
            };

            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
                .Update
                .Set(propertyPath, data);

            var result = await _users.UpdateOneAsync(filter, update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindById(id, clientId);
            throw new Exception("User could not be updated");
        }

        public async Task<TAuthUser> UpdateUser<Type>(string propertyPath, Type data, string id, string clientId)
        {
            var filter = new BsonDocument
            {
                { "ClientId", new BsonDocument { { "_id", clientId }}},
                { "_id", id}
            };

            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
                .Update
                .Set(propertyPath, data);

            var result = await _users.UpdateOneAsync(filter, update);
            if (result.IsAcknowledged) return await FindById(id, clientId);
            throw new Exception("User could not be updated");
        }

        #region Invitations

        public async Task<TAuthUser> SetInvitation(string userId, AuthUserInvitation invite, string clientId)
        {
            var user = await FindById(userId, clientId);
            if (user == null) throw new Exception("User could not be found");

            var update = Builders<TAuthUser>
                .Update
                .Set("Invitation", invite);
            var result = await _users.UpdateOneAsync(FilterById(user.Id, clientId), update);
            return await FindById(userId, clientId);
        }

        public async Task<TAuthUser> GetUserByInvitationKey(string invitationKey, string clientId)
        {
            var filter = new BsonDocument
            {
                { "ClientId._id",  clientId},
                { "Invitation.InvitationKey", invitationKey}
            };

            var json = filter.ToJson();

            var result = await _users.OfType<TAuthUser>().Find(filter).FirstOrDefaultAsync();
            return result;
        }

        public async Task<TAuthUser> SetInvitationStatus(string userId, AuthUserInvitationStatus status, string clientId)
        {
            UpdateDefinition<TAuthUser> update = Builders<TAuthUser>
               .Update
               .Set("Invitation.InvitationStatus", status);

            var result = await _users.UpdateOneAsync(FilterById(userId, clientId), update);
            if (result.IsAcknowledged && result.ModifiedCount > 0) return await FindById(userId, clientId);
            throw new Exception("Invitation status could not be updated");

        }

        #endregion

        private BsonDocument FilterByEmail(string email, string clientId)
        {
            var normEmail = _normaliser.Normalise(email);
            return new BsonDocument
            {
                { "ClientId._id",clientId }, { "NormalisedEmail", normEmail}
            };
        }

        private BsonDocument FilterById(string id, string clientId)
        {
            return new BsonDocument
            {
                { "ClientId._id",clientId }, { "_id", id}
            };
        }


    }
}
