namespace TechDevs.Accounts
{
    public class AuthUser
    {
        public string Id { get; set; }
        public DBRef ClientId { get; set; }
        public string Username { get; set; }
        public string NormalisedUsername { get; set; }
        public string EmailAddress { get; set; }
        public string NormalisedEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AgreedToTerms { get; set; }
        public bool ValidatedEmail { get; set; }
        public string PasswordHash { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
    }
}