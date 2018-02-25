using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechDevs.Core.UserManagement
{
    public interface IUserService
    {
        Task<List<IUser>> GetAllUsers();
        Task<IUser> Create(IUserRegistration registration);
        Task<IUser> GetByEmail(string email);
        Task<IUser> UpdateEmail(string currentEmail, string newEmail);
        Task Delete(string email);
        Task SetPassword(string email, string password);
        Task RequestResetPassword(string email);
        Task ResetPassword(string email, string resetPasswordToken);
        Task<bool> ValidatePassword(string email, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
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
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                EmailAddress = userRegistration.EmailAddress,
                AgreedToTerms = userRegistration.AggreedToTerms
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
            var updatedUser = await _userRepo.SetEmail(user, newEmail);
            if (updatedUser == null) throw new Exception("Email update failed");
            return updatedUser;
        }

        public async Task<IUser> Create(IUserRegistration registration)
        {
            await ValidateCanRegister(registration);
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                EmailAddress = registration.EmailAddress,
                AgreedToTerms = registration.AggreedToTerms
            };
            var result = await _userRepo.Insert(user);
            if (result == null) throw new Exception("User creation failed");
            return result;
        }

        public async Task Delete(string email)
        {
            var user = await _userRepo.FindByEmail(email);
            if (user == null) throw new Exception("User not found");
            await _userRepo.Delete(user);
        }

        public async Task<IUser> GetByEmail(string email)
        {
            var user = await _userRepo.FindByEmail(email);
            if (user == null) throw new Exception("User not found");
            return user;
        }

        public Task SetPassword(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task RequestResetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public Task ResetPassword(string email, string resetPasswordToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidatePassword(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}