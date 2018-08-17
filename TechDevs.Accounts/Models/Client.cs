using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TechDevs.Accounts
{
    [BsonDiscriminator("Client")]
    [BsonIgnoreExtraElements]
    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BusinessTitle { get; set; }
        public List<IEmployee> Employees { get; set; }
        public List<ICustomer> Customers { get; set; }
        public ClientTheme CustomerPortalTheme { get; set; }
    }

    public class ClientTheme
    {
        public string Font { get; set; }
        public string PrimaryColour { get; set; }
        public string SecondaryColour { get; set; }
        public string WarningColour { get; set; }
        public string DangerColour { get; set; }
        public string LogoPath { get; set; }
        public byte[] LogoData { get; set; }
    }
}