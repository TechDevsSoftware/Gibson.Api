using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.NotificationPreferences
{
    public interface INotificationPreferencesService
    {
        Task<Customer> UpdateNotificationPreferences(CustomerNotificationPreferences notificationPreferences, string userId, string clientId);
    }
}