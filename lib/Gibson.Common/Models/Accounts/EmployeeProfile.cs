namespace Gibson.Common.Models
{
    public class EmployeeProfile : AuthUserProfile
    {
        public CustomerData CustomerData { get; set; }

        public EmployeeProfile(Employee emp)
        {
            Username = emp.Username;
            FirstName = emp.FirstName;
            LastName = emp.LastName;
            EmailAddress = emp.EmailAddress;
            AgreedToTerms = emp.AgreedToTerms;
            ProviderName = emp.ProviderName;
            ContactNumber = emp.ContactNumber;
        }
    }
}