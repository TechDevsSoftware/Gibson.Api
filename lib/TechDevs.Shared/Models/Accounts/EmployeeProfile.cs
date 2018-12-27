namespace TechDevs.Shared.Models
{
    public class EmployeeProfile : AuthUserProfile
    {
        public string Status {get;set;}
        public EmployeeData EmployeeData { get; set; }
        public EmployeeProfile(Employee emp)
        {
            Username = emp.Username;
            FirstName = emp.FirstName;
            LastName = emp.LastName;
            EmailAddress = emp.EmailAddress;
            AgreedToTerms = emp.AgreedToTerms;
            ValidatedEmail = emp.ValidatedEmail;
            EmployeeData = emp.EmployeeData;
            ProviderName = emp.ProviderName;

            if (emp.Disabled == false) Status = "Active";
            else if (emp.Invitation == null) Status = "Disabled";
            else Status = $"Invited - {emp?.Invitation?.Status.Value}";
        }
    }
}