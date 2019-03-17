using System.Threading.Tasks;
using Gibson.Common.Models;

namespace TechDevs.Clients
{
    public interface IClientThemeService
    {
        Task<Client> SetParameter(string clientId, string key, string value);
        Task<Client> RemoveParameter(string clientId, string key);
    }
}
