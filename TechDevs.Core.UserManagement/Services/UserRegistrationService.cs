using System.Text;
using System.Text.RegularExpressions;

namespace TechDevs.Core.UserManagement
{
    public class UserRegistrationService
    {
        IUserRepository _userRepo;

        public UserRegistrationService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public IUser RegisterUser(IUserRegistration userRegistration)
        {
            ValidateCanRegister(userRegistration);
            return new User();
        }

        void ValidateCanRegister(IUserRegistration userRegistration)
        {
            var validationErrors = new StringBuilder();

            // Email address cannot already exist
            if (_userRepo.UserByEmail(userRegistration.EmailAddress) != null)
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