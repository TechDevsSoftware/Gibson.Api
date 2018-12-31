namespace TechDevs.Shared.Models
{
    public class MarketingNotificationPreferences
    {
        public bool SMS { get; set; }
        public bool Email { get; set; }
        public bool Phone { get; set; }
        public bool Post { get; set; }

        public MarketingNotificationPreferences()
        {
            SMS = true;
            Phone = true;
            Post = true;
            Email = true;
        }
    }
}