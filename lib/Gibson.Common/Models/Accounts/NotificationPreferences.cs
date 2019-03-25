using System.Collections.Generic;

namespace Gibson.Common.Models
{
    public class NotificationPreferences
    {
        public bool Email { get; set; }
        public bool SMS { get; set; }
        public bool PushNotifications { get; set; }


        public NotificationPreferences()
        {
            Email = true;
            SMS = true;
            PushNotifications = true;
        }
    }
}