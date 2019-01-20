namespace TechDevs.Shared.Models
{
    public class PublicClient
    {
        public string Name { get; set; }
        public string ShortKey { get; set; }
        public string LogoPath { get; set; }

        public PublicClient(Client client)
        {
            Name = client.Name;
            ShortKey = client.ShortKey;
            LogoPath = client?.ClientTheme?.LogoPath;
        }
    }
}