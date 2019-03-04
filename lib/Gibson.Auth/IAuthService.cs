using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.Auth
{
    public interface IAuthService
    {
        Task<string> Login(LoginRequest req);
    }
}