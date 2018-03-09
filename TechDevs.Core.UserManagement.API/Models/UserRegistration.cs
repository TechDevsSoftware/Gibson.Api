namespace TechDevs.Core.UserManagement
{
    public interface IUserRegistration
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string EmailAddress { get; set; }
        bool AggreedToTerms { get; set; }
    }

    public class UserRegistration : IUserRegistration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AggreedToTerms { get; set; }
    }
}