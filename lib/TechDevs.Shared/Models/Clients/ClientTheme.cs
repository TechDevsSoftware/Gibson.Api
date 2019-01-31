using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class ClientTheme
    {
        public string Font { get; set; }
        public string PrimaryColour { get; set; }
        public string SecondaryColour { get; set; }
        public string WarningColour { get; set; }
        public string DangerColour { get; set; }
        public string LogoPath { get; set; }
        public byte[] LogoData { get; set; }
        public List<CSSParameter> CssParameters { get; set; }
    }
}