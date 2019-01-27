using System;
namespace TechDevs.Shared.Models
{
    public class ClientIdentifier
    {
        public string Id { get; set; }
        public string ShortKey { get; set; }
        public string Name { get; set; }

        public static explicit operator ClientIdentifier(Client v)
        {
            throw new NotImplementedException();
        }
    }
}
