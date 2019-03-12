using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechDevs.Shared;
using TechDevs.Shared.Models;

namespace Gibson.Users
{
    public class UserRegistrationService : IUserRegistrationService
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
                UserType = reg.UserType,
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
                },
                Enabled = true
            };
            var result = await repo.Create(newUser, clientId);
            return result;
        }
        
        private async Task ValidateCanRegister(UserRegistration reg, Guid clientId)
        {
            if(string.IsNullOrEmpty(reg.EmailAddress)) throw new ArgumentNullException(reg.EmailAddress);
            
            var validationErrors = new List<string>();
            
            // Check required fields
            if (string.IsNullOrEmpty(reg.FirstName)) validationErrors.Add("First name is required");
            if (string.IsNullOrEmpty(reg.LastName)) validationErrors.Add("Last name is required");
            
            // User must have agreed to the terms
            if (!reg.AggreedToTerms) validationErrors.Add("Must agree to terms and conditions");

            // Email address cannot already exist
            var existingUser = await repo.GetUserByUserName(reg.EmailAddress,  reg.UserType, clientId);
            if (existingUser != null) validationErrors.Add("Username has already been registered");

            // Email address must be valid format
            const string emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (!Regex.IsMatch(reg.EmailAddress, emailRegex, RegexOptions.IgnoreCase))
                validationErrors.Add("Not a valid email address");
       
            // Must have a supported auth provider
            if(reg.ProviderName != "Gibson" && reg.ProviderName != "Google") 
                validationErrors.Add("Auth provider not supported");
            
            // Must have a set user type
            if(reg.UserType == GibsonUserType.NotSet)
                validationErrors.Add("User type is not set");
            
            if (validationErrors.Count > 0)
                throw new UserRegistrationException(reg, validationErrors, "Registration validation failed");
        }
    }
}