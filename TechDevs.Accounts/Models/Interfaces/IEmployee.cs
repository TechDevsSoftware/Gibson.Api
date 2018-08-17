namespace TechDevs.Accounts
{
    public interface IEmployee : IAuthUser
    {
        string Role { get; set; }
        string JobTitle { get; set; }
    }
}