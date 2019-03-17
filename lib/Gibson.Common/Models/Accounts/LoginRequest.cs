using System;

namespace Gibson.Common.Models
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProviderIdToken { get; set; }
    }
}