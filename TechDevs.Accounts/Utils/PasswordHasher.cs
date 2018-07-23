using Microsoft.Extensions.Options;
using System;

namespace TechDevs.Accounts
{
    public class BCryptPasswordHasherOptions
    {
        public int WorkFactor { get; set; } = 10;
        public bool EnhancedEntropy { get; set; } = false;
    }

    public class BCryptPasswordHasher : IPasswordHasher
    {
        private readonly BCryptPasswordHasherOptions options;

        public BCryptPasswordHasher(IOptions<BCryptPasswordHasherOptions> optionsAccessor = null)
        {
            options = optionsAccessor?.Value ?? new BCryptPasswordHasherOptions();
        }

        public string HashPassword(IUser user, string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password, options.WorkFactor, options.EnhancedEntropy);
        }

        public bool VerifyHashedPassword(IUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));
            if (providedPassword == null) throw new ArgumentNullException(nameof(providedPassword));

            var isValid = BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword, options.EnhancedEntropy);

            return isValid;
        }
    }
}
