using System.Threading.Tasks;
using TechDevs.Accounts;
using TechDevs.Shared.Models;

namespace TechDevs.NotificationPreferences
{
    public class NotificationPreferencesService : INotificationPreferencesService
    {
        private readonly ICustomerService _customerService;

        public NotificationPreferencesService(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<Customer> UpdateNotificationPreferences(CustomerNotificationPreferences notificationPreferences, string userId, string clientId)
        {
            var result = await _customerService.UpdateCustomerData(userId, "NotificationPreferences", notificationPreferences, clientId);
            return result;
        }
    }
}
