using Microsoft.AspNetCore.Identity;

namespace TechDevs.Core.UserManagement
{
    public class User : IUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string NormalisedUsername { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string NormalisedEmail { get; set; }
        public bool AgreedToTerms { get; set; }
        public bool ValidatedEmail { get; set; }
        public string PasswordHash { get; set; }
    }
}