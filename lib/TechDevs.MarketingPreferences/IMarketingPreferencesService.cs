using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.MarketingPreferences
{
    public interface IMarketingPreferencesService
    {
        Task<Customer> UpdateMarketingPreferences(MarketingNotificationPreferences marketingPreferences, string userId, string clientId);
    }
}