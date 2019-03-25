using System;

namespace Gibson.Common.Models
{
    public class PublicClient
    {
        public string Name { get; set; }
        public string ShortKey { get; set; }
        public Guid Id { get; set; }
        
        public ClientTheme Theme { get; set; }

        public PublicClient(Client client)
        {
            Id = Guid.Parse(client.Id);
            Name = client.Name;
            ShortKey = client.ShortKey;
            Theme = client?.ClientTheme;
        }
    }
}