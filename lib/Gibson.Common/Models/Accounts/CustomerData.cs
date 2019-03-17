using System.Collections.Generic;

namespace Gibson.Common.Models
{
    public class CustomerData
    {
        public List<CustomerVehicle> MyVehicles { get; set; }
        public CustomerNotificationPreferences NotificationPreferences { get; set; }
        public MarketingNotificationPreferences MarketingPreferences { get; set; }

        public CustomerData()
        {
            MyVehicles = new List<CustomerVehicle>();
            NotificationPreferences = new CustomerNotificationPreferences();
            MarketingPreferences = new MarketingNotificationPreferences();
        }
    }
}