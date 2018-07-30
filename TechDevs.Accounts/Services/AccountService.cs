using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechDevs.Accounts
{


    public class AccountService : IAccountService
    {
        readonly IUserRepository _userRepo;
        readonly IPasswordHasher _passwordHasher;

        public AccountService(IUserRepository userRepo, IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<IUser>> GetAllUsers()
        {
            var result = await _userRepo.GetAll();
            return result;
        }

        public async Task<IUser> RegisterUser(IUserRegistration userRegistration)
        {
            await ValidateCanRegister(userRegistration);
            var newUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                EmailAddress = userRegistration.EmailAddress,
                Username = userRegistration.EmailAddress,
                AgreedToTerms = userRegistration.AggreedToTerms,
                ProviderId = userRegistration.ProviderId ?? "TechDevs",
                ProviderName = userRegistration.ProviderName ?? "TechDevs",
                UserData = new UserData()
            };
            var result = await _userRepo.Insert(newUser);
            return result;
        }

        public async Task ValidateCanRegister(IUserRegistration userRegistration)
        {
            var validationErrors = new StringBuilder();

            // User must have agreed to the terms
            if (!userRegistration.AggreedToTerms)
                validationErrors.Append("Must agree to terms and conditions");

            // Email address cannot already exist
            if (await _userRepo.UserExists(userRegistration.EmailAddress))
                validationErrors.AppendLine("Email address has already been registered");

            // Email address must be valid format
            const string emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (!Regex.IsMatch(userRegistration.EmailAddress, emailRegex, RegexOptions.IgnoreCase))
                validationErrors.AppendLine("Email address is invalid");

            // Check required fields
            if (string.IsNullOrEmpty(userRegistration.FirstName)) validationErrors.AppendLine("First name is required");
            if (string.IsNullOrEmpty(userRegistration.LastName)) validationErrors.Append("Last name is required");
            if (string.IsNullOrEmpty(userRegistration.EmailAddress)) validationErrors.Append("Email address is required");

            if (validationErrors.Length > 0)
                throw new UserRegistrationException(userRegistration, validationErrors.ToString());
        }

        public async Task<IUser> UpdateEmail(string currentEmail, string newEmail)
        {
            // Get user
            var user = await _userRepo.FindByEmail(currentEmail);
            if (user == null) throw new Exception("User not found");
            // Check that the new email is not already taken
            var existingEmail = await _userRepo.FindByEmail(newEmail);
            if (existingEmail != null) throw new Exception("Email address already in use");
            // Change the email
            await _userRepo.SetUsername(user, newEmail);
            var updatedUser = await _userRepo.SetEmail(user, newEmail);
            if (updatedUser == null) throw new Exception("Email update failed");
            // Set the username along with the username as we dont need a seperate username
            await _userRepo.SetUsername(user, newEmail);
            return updatedUser;
        }

        public async Task<bool> Delete(string email)
        {
            var user = await _userRepo.FindByEmail(email);
            if (user == null) throw new Exception("User not found");
            return await _userRepo.Delete(user);
        }

        public async Task<IUser> GetByEmail(string email)
        {
            var user = await _userRepo.FindByEmail(email);
            return user;
        }

        public async Task<IUser> GetByProvider(string provider, string providerId)
        {
            var user = await _userRepo.FindByProvider(provider, providerId);
            return user;
        }

        public async Task<IUser> SetPassword(string email, string password)
        {
            var user = await _userRepo.FindByEmail(email);
            if (user == null) throw new Exception("User not found");
            var hashedPassword = _passwordHasher.HashPassword(user, password);
            var result = await _userRepo.SetPassword(user, hashedPassword);
            return result;
        }

        public Task RequestResetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task ResetPassword(string email, string resetPasswordToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidatePassword(string email, string password)
        {
            try
            {
                var user = await _userRepo.FindByEmail(email);
                if (user == null) return false;
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IUser> GetById(string id)
        {
            var user = await _userRepo.FindById(id);
            return user;
        }
    }

}