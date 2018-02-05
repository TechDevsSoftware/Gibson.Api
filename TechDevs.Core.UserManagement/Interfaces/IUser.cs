namespace TechDevs.Core.UserManagement
{
    public interface IUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string EmailAddress { get; set; }
        string VehicleRegistration { get; set; }
    }
}