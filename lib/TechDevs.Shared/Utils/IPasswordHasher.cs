using TechDevs.Shared.Models;

namespace TechDevs.Shared
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
