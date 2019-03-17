namespace Gibson.Common.Models
{
    public class CustomerProfile : AuthUserProfile
    {
        public CustomerData CustomerData { get; set; }

        public CustomerProfile(Customer cust)
        {
            CustomerData = cust.CustomerData;

            Username = cust.Username;
            FirstName = cust.FirstName;
            LastName = cust.LastName;
            EmailAddress = cust.EmailAddress;
            AgreedToTerms = cust.AgreedToTerms;
            ProviderName = cust.ProviderName;
            ContactNumber = cust.ContactNumber;

        }
    }
}