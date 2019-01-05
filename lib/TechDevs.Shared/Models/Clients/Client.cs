using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SiteUrl { get; set; }
        public string ClientApiKey { get; set; }
        public string ShortKey { get; set; }
        public List<AuthUserProfile> Employees { get; set; }
        public List<AuthUserProfile> Customers { get; set; }
        public ClientTheme ClientTheme { get; set; }
        public ClientData ClientData { get; set; }

        public Client()
        {
            ClientTheme = new ClientTheme();
            ClientData = new ClientData();
            Employees = new List<AuthUserProfile>();
            Customers = new List<AuthUserProfile>();
        }
    }
    
    public class ClientData
    {
        public List<BasicOffer> BasicOffers { get; set; }

        public ClientData()
        {
            BasicOffers = new List<BasicOffer>();
        }
    }
}