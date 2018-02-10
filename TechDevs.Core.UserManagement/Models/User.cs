using TechDevs.Core.UserManagement.Interfaces;

namespace TechDevs.Core.UserManagement.Models
{
    public class User : IUser
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AgreedToTerms { get; set; }
    }
}