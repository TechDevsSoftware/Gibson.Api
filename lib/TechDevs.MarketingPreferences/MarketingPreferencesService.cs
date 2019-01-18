using System.Threading.Tasks;
using TechDevs.Customers;
using TechDevs.Shared.Models;

namespace TechDevs.MarketingPreferences
{
    public class MarketingPreferencesService : IMarketingPreferencesService
    {
        private readonly ICustomerService _customerService;

        public MarketingPreferencesService(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<Customer> UpdateMarketingPreferences(MarketingNotificationPreferences marketingPreferences, string userId, string clientId)
        {
            var result = await _customerService.UpdateCustomerData(userId, "MarketingPreferences", marketingPreferences, clientId);
            return result;
        }
    }
}
