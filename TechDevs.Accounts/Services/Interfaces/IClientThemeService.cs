using System.Threading.Tasks;

namespace TechDevs.Accounts.Services
{
    public interface IClientThemeService
    {
        Task<Client> SetParameter(string clientId, string key, string value);
        Task<Client> RemoveParameter(string clientId, string key);
    }
}
