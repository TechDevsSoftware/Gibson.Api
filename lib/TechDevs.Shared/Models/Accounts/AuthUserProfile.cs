namespace TechDevs.Shared.Models
{
    public abstract class AuthUserProfile
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AgreedToTerms { get; set; }
        public bool ValidatedEmail { get; set; }
        public string ProviderName { get; set; }
    }
}