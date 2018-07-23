namespace TechDevs.Accounts
{
    public interface IPasswordHasher
    {
        string HashPassword(IUser user, string password);
        bool VerifyHashedPassword(IUser user, string hashedPassword, string providedPassword);
    }
}
