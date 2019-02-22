using System;

namespace TechDevs.Shared.Models
{
    public class LoginRequest
    {
        public string Provider { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProviderIdToken { get; set; }
        public string ClientKey { get; set; }
        public Guid ClientId { get; set; }
    }
}