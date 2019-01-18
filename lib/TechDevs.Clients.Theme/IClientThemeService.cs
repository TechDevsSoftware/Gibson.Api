using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Clients
{
    public interface IClientThemeService
    {
        Task<Client> SetParameter(string clientId, string key, string value);
        Task<Client> RemoveParameter(string clientId, string key);
    }
}
