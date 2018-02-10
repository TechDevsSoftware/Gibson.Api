namespace TechDevs.Core.UserManagement.Interfaces
{
    public interface IUser
    {
        string UserId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string EmailAddress { get; set; }
        bool AgreedToTerms { get; set; }
    }
}