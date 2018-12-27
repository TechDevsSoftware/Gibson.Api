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
        public List<EmployeeProfile> Employees { get; set; }
        public List<CustomerProfile> Customers { get; set; }
        public ClientTheme ClientTheme { get; set; }
    }
}