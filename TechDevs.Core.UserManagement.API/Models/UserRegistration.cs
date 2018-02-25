namespace TechDevs.Core.UserManagement
{
    public class UserRegistration : IUserRegistration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AggreedToTerms { get; set; }
    }
}