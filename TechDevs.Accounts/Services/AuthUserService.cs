using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechDevs.Accounts.Repositories;

namespace TechDevs.Accounts
{
    public class CustomerService : AuthUserService<Customer>
    {
        public CustomerService(IAuthUserRepository<Customer> userRepo, IPasswordHasher passwordHasher) : base(userRepo, passwordHasher)
        {
        }
    }

    public class EmployeeService : AuthUserService<Employee>
    {
        public EmployeeService(IAuthUserRepository<Employee> userRepo, IPasswordHasher passwordHasher) : base(userRepo, passwordHasher)
        {
        }
    }

    public abstract class AuthUserService<TAuthUser> : IAuthUserService<TAuthUser> where TAuthUser : IAuthUser, new()
    {
        readonly IAuthUserRepository<TAuthUser> _userRepo;
        readonly IPasswordHasher _passwordHasher;

        public AuthUserService(IAuthUserRepository<TAuthUser> userRepo, IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<TAuthUser>> GetAllUsers(string clientId)
        {
            var result = await _userRepo.FindAll(clientId);
            return result;
        }

        public async Task<TAuthUser> RegisterUser(TAuthUser newUser, IAuthUserRegistration userRegistration, string clientId)
        {
            await ValidateCanRegister(userRegistration, clientId);

            var newAuthUser = new TAuthUser
            {
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                EmailAddress = userRegistration.EmailAddress,
                AgreedToTerms = userRegistration.AggreedToTerms,
                ProviderName = userRegistration.ProviderName,
                ProviderId = userRegistration.ProviderId
            };

            var result = await _userRepo.Insert(newAuthUser, clientId);
            var resultAfterPassword = await SetPassword(result.EmailAddress, userRegistration.Password, clientId);
            return resultAfterPassword;
        }

        public async Task ValidateCanRegister(IAuthUserRegistration userRegistration, string clientId)
        {
            var validationErrors = new StringBuilder();

            // User must have agreed to the terms
            if (!userRegistration.AggreedToTerms)
                validationErrors.Append("Must agree to terms and conditions");

            // Email address cannot already exist
            if (await _userRepo.UserExists(userRegistration.EmailAddress, clientId))
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

        public async Task<TAuthUser> UpdateEmail(string currentEmail, string newEmail, string clientId)
        {
            // Get user
            var user = await _userRepo.FindByEmail(currentEmail, clientId);
            if (user == null) throw new Exception("User not found");
            // Check that the new email is not already taken
            var existingEmail = await _userRepo.FindByEmail(newEmail, clientId);
            if (existingEmail != null) throw new Exception("Email address already in use");
            // Change the email
            await _userRepo.SetUsername(user, newEmail, clientId);
            var updatedUser = await _userRepo.SetEmail(user, newEmail, clientId);
            if (updatedUser == null) throw new Exception("Email update failed");
            // Set the username along with the username as we dont need a seperate username
            await _userRepo.SetUsername(user, newEmail, clientId);
            return updatedUser;
        }

        public async Task<bool> Delete(string email, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            if (user == null) throw new Exception("User not found");
            return await _userRepo.Delete(user, clientId);
        }

        public async Task<TAuthUser> GetByEmail(string email, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            return user;
        }

        public async Task<TAuthUser> GetByProvider(string provider, string providerId, string clientId)
        {
            var user = await _userRepo.FindByProvider(provider, providerId, clientId);
            return user;
        }

        public async Task<TAuthUser> SetPassword(string email, string password, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            if (user == null) throw new Exception("User not found");
            var hashedPassword = _passwordHasher.HashPassword(user, password);
            var result = await _userRepo.SetPassword(user, hashedPassword, clientId);
            return result;
        }

        public Task RequestResetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task ResetPassword(string email, string resetPasswordToken, string clientId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ValidatePassword(string email, string password, string clientId)
        {
            try
            {
                var user = await _userRepo.FindByEmail(email, clientId);
                if (user == null) return false;
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<TAuthUser> GetById(string id, string clientId)
        {
            var user = await _userRepo.FindById(id, clientId);
            return user;
        }
    }

}