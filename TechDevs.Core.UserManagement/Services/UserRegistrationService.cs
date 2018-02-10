using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechDevs.Core.UserManagement.Interfaces;

namespace TechDevs.Core.UserManagement.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserRepository _userRepo;

        public UserRegistrationService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IUser> RegisterUser(IUserRegistration userRegistration)
        {
            await ValidateCanRegister(userRegistration);
            var result = await _userRepo.CreateUser(userRegistration);
            return result;
        }

        public async Task ValidateCanRegister(IUserRegistration userRegistration)
        {
            var validationErrors = new StringBuilder();

            // User must have agreed to the terms
            if (!userRegistration.AggreedToTerms)
                validationErrors.Append("Must agree to terms and conditions");

            // Email address cannot already exist
            if (await _userRepo.EmailAlreadyRegistered(userRegistration.EmailAddress))
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
    }
}