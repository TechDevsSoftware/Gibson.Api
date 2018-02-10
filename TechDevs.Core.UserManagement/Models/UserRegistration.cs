using TechDevs.Core.UserManagement.Interfaces;

namespace TechDevs.Core.UserManagement.Models
{
    public class UserRegistration : IUserRegistration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AggreedToTerms { get; set; }
    }
}