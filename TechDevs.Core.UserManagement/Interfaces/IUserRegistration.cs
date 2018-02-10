namespace TechDevs.Core.UserManagement.Interfaces
{
    public interface IUserRegistration
    {
        // Required
        string FirstName { get; set; }
        string LastName { get; set; }
        string EmailAddress { get; set; }
        bool AggreedToTerms { get; set; }
    }
}
