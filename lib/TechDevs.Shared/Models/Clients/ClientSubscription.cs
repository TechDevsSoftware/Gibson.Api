using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class ClientSubscription
    {
        public Product ActivePackage { get; set; }
        public List<Product> ActiveBoltOns { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        
        public ClientSubscription()
        {
            SubscriptionStatus =   SubscriptionStatus.None;  
            ActiveBoltOns = new List<Product>();
        }
    }
}