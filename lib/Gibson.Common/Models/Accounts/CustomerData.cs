using System.Collections.Generic;

namespace Gibson.Common.Models
{
    public class CustomerData
    {
        public List<CustomerVehicle> MyVehicles { get; set; }
        public NotificationPreferences NotificationPreferences { get; set; }
        public MarketingPreferences MarketingPreferences { get; set; }

        public CustomerData()
        {
            MyVehicles = new List<CustomerVehicle>();
            NotificationPreferences = new NotificationPreferences();
            MarketingPreferences = new MarketingPreferences();
        }
    }
}