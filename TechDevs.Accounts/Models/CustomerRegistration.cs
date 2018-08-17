namespace TechDevs.Accounts
{
    public interface IAuthUserRegistration
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string EmailAddress { get; set; }
        bool AggreedToTerms { get; set; }
        string ProviderName { get; set; }
        string ProviderId { get; set; }
        string Password { get; set; }
    }

    public class AuthUserRegistration : IAuthUserRegistration
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AggreedToTerms { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string Password { get; set; }
    }
}