namespace TechDevs.Shared.Models
{
    public class CustomerProfile : AuthUserProfile
    {
        public CustomerData CustomerData { get; set; }
        public CustomerProfile(Customer user)
        {
            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAddress = user.EmailAddress;
            AgreedToTerms = user.AgreedToTerms;
            ValidatedEmail = user.ValidatedEmail;
            CustomerData = user.CustomerData;
        }
    }

}