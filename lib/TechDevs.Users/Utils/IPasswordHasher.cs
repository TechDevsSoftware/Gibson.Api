using TechDevs.Shared.Models;

namespace TechDevs.Users
{
    public interface IPasswordHasher
    {
        string HashPassword(AuthUser user, string password);
        bool VerifyHashedPassword(AuthUser user, string hashedPassword, string providedPassword);
    }
}
