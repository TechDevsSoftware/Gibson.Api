using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class CustomerNotificationPreferences
    {
        public bool Email { get; set; }
        public bool SMS { get; set; }
        public bool PushNotifications { get; set; }


        public CustomerNotificationPreferences()
        {
            Email = true;
            SMS = true;
            PushNotifications = true;
        }
    }
}