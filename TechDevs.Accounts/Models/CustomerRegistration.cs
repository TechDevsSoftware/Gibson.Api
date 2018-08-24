namespace TechDevs.Accounts
{
    public class AuthUserRegistration
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