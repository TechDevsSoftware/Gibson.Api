namespace TechDevs.Core.UserManagement
{
    public interface IUserRepository
    {
        IUser UserByEmail(string emailAddress);
    }
}