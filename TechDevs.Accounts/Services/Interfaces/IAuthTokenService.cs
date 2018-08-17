using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public interface IAuthTokenService
    {
        Task<string> CreateToken(string userId, string requestedClaims, string clientId);
    }
}