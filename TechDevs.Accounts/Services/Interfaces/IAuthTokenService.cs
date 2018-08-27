using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public interface IAuthTokenService<TAuthUser> where TAuthUser : AuthUser, new()
    {
        Task<string> CreateToken(string userId, string requestedClaims, string clientId);
    }
}