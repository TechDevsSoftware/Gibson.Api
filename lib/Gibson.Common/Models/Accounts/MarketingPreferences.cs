namespace Gibson.Common.Models
{
    public class MarketingPreferences
    {
        public bool SMS { get; set; }
        public bool Email { get; set; }
        public bool Phone { get; set; }
        public bool Post { get; set; }

        public MarketingPreferences()
        {
            SMS = true;
            Phone = true;
            Post = true;
            Email = true;
        }
    }
}