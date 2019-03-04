using System;
using System.Threading.Tasks;
using Gibson.Shared.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechDevs.Shared.Models;

namespace Gibson.Users
{
    public class UserRepository : ClientDataRepository<User>, IUserRepository
    {
        public UserRepository(string collectionName, IOptions<MongoDbSettings> dbSettings) : base(collectionName, dbSettings)
        {
        }

        public async Task<User> GetUserByUserName(string username, GibsonUserType userType, Guid clientId)
        {
            var results = await collection.FindAsync(x => x.Username == username && x.ClientId == clientId && x.UserType == userType);
            return await results.FirstOrDefaultAsync();
        }
        
        public async Task<User> GetUserByProviderId(string providerId, GibsonUserType userType, Guid clientId)
        {
            var results = await collection.FindAsync(x => x.AuthProfile.ProviderId == providerId && x.ClientId == clientId && x.UserType == userType);
            return await results.FirstOrDefaultAsync();
        }
    }
}