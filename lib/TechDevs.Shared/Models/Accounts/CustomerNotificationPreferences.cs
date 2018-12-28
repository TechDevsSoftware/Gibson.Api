using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class CustomerNotificationPreferences
    {
        public bool MotEmail { get; set; }
        public bool MotPush { get; set; }
        public bool ServiceEmail { get; set; }
        public bool ServicePush { get; set; }
        public bool OffersEmail { get; set; }
        public bool OffersPush { get; set; }

        public CustomerNotificationPreferences()
        {
            MotEmail = true;
            MotPush = true;
            ServiceEmail = true;
            ServicePush = true;
            OffersEmail = true;
            OffersPush = true;
        }
    }
}