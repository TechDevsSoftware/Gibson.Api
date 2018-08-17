namespace TechDevs.Accounts
{
    public interface IPasswordHasher
    {
        string HashPassword(IAuthUser user, string password);
        bool VerifyHashedPassword(IAuthUser user, string hashedPassword, string providedPassword);
    }
}
