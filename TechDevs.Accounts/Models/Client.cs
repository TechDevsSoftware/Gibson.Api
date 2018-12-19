using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TechDevs.Accounts
{
    public class ClientRegistration
    {
        public string Name { get; set; }
        public string SiteUrl { get; set; }
        public string ShortKey { get; set; }
    }

    [BsonDiscriminator("Client")]
    [BsonIgnoreExtraElements]
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

    [BsonIgnoreExtraElements]
    public class ClientTheme
    {
        public string Font { get; set; }
        public string PrimaryColour { get; set; }
        public string SecondaryColour { get; set; }
        public string WarningColour { get; set; }
        public string DangerColour { get; set; }
        public string LogoPath { get; set; }
        public byte[] LogoData { get; set; }
        public List<CSSParameter> Parameters { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CSSParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

}