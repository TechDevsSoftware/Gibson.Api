namespace TechDevs.Shared.Models
{
    public class AuthUserProfile
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AgreedToTerms { get; set; }
        public bool ValidatedEmail { get; set; }
        public string ProviderName { get; set; }
        public string ContactNumber { get; set; }

        public AuthUserProfile() { }
        public AuthUserProfile(Customer c)
        {
            Username = c.Username;
            FirstName = c.FirstName;
            LastName = c.LastName;
            EmailAddress = c.EmailAddress;
            AgreedToTerms = c.AgreedToTerms;
            ProviderName = c.ProviderName;
            ContactNumber = c.ContactNumber;
        }

        public AuthUserProfile(Employee e)
        {
            Username = e.Username;
            FirstName = e.FirstName;
            LastName = e.LastName;
            EmailAddress = e.EmailAddress;
            AgreedToTerms = e.AgreedToTerms;
            ProviderName = e.ProviderName;
            ContactNumber = e.ContactNumber;
        }
    }
}