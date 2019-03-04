using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gibson.Shared.Repositories;
using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using MongoDB.Driver;
using TechDevs.Users;

namespace Gibson.Users
{
    public class UserRepository : ClientDataRepository<User>, IUserRepository
    {
        public UserRepository(string collectionName, IOptions<MongoDbSettings> dbSettings) : base(collectionName, dbSettings)
        {
        }

        public async Task<User> GetUserByUserName(string username, Guid clientId)
        {
            var results = await collection.FindAsync(x => x.Username == username && x.ClientId == clientId);
            return await results.FirstOrDefaultAsync();
        }
    }
    
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUserName(string username, Guid clientId);
    }

    public class UserService
    {
        private readonly IUserRepository repo;

        public UserService(IUserRepository repo)
        {
            this.repo = repo;
        }

        public async Task<User> FindByUsername(string username, Guid clientId)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (clientId == Guid.Empty) throw new ArgumentNullException(nameof(clientId));
            var result = await repo.GetUserByUserName(username, clientId);
            return result;
        }

        public async Task<User> FindById(Guid id, Guid clientId)
        {
            if (id == Guid.Empty) throw new Exception("UserId is empty");
            if (clientId == Guid.Empty) throw new Exception("ClientId is empty");
            var result = await repo.FindById(id, clientId);
            return result;
        }
    }

    public class UserRegistrationService
    {
        private readonly IUserRepository repo;
        private readonly IPasswordHasher _hasher;

        public UserRegistrationService(IUserRepository repo, IPasswordHasher hasher)
        {
            this.repo = repo;
            _hasher = hasher;
        }
        
        public async Task<User> RegisterUser(UserRegistration reg, Guid clientId)
        {
            if (clientId == Guid.Empty) throw new ArgumentNullException(nameof(clientId));
            await ValidateCanRegister(reg, clientId);
            var newUser = new User
            {
                Username =  reg.EmailAddress,
                UserProfile = new UserProfile
                {
                    FirstName = reg.FirstName,
                    LastName = reg.LastName,
                    Email = reg.EmailAddress,
                },
                AuthProfile = new AuthProfile
                {
                    AuthProvider = (reg.ProviderName == "Google") ? GibsonAuthProvider.Google : GibsonAuthProvider.Gibson,
                    ProviderId = (reg.ProviderName == "Google") ? reg.ProviderId : null,
                    PasswordHash = (reg.ProviderName == "Gibson") ? _hasher.HashPassword(reg.Password) : null
                }
            };
            var result = await repo.Create(newUser, clientId);
            return result;
        }
        
        private async Task ValidateCanRegister(UserRegistration userRegistration, Guid clientId)
        {
            if(string.IsNullOrEmpty(userRegistration.EmailAddress)) throw new ArgumentNullException(userRegistration.EmailAddress);
            
            var validationErrors = new List<string>();
            
            // Check required fields
            if (string.IsNullOrEmpty(userRegistration.FirstName)) validationErrors.Add("First name is required");
            if (string.IsNullOrEmpty(userRegistration.LastName)) validationErrors.Add("Last name is required");
            
            // User must have agreed to the terms
            if (!userRegistration.AggreedToTerms) validationErrors.Add("Must agree to terms and conditions");

            // Email address cannot already exist
            var existingUser = await repo.GetUserByUserName(userRegistration.EmailAddress, clientId);
            if (existingUser != null) validationErrors.Add("Username has already been registered");

            // Email address must be valid format
            const string emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (!Regex.IsMatch(userRegistration.EmailAddress, emailRegex, RegexOptions.IgnoreCase))
                validationErrors.Add("Not a valid email address");
           

            if (validationErrors.Count > 0)
                throw new UserRegistrationException(userRegistration, validationErrors, "Registration validation failed");
        }
    }
}
