using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Gibson.Auth.Tokens
{
    public static class AuthTokenExtentions
    {
        public static Guid GetUserId(this string token)
        {
            var jwt = token.ToJwtToken();
            if (jwt == null) return Guid.Empty;
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            return Guid.Parse(userId);
        }

        public static string GetClientKey(this string token)
        {
            var jwt = token.ToJwtToken();
            if (jwt == null) return null;
            var clientKey = jwt.Claims.FirstOrDefault(c => c.Type == "Gibson-ClientKey")?.Value;
            return clientKey;
        }

        public static Guid GetClientId(this string token)
        {
            var jwt = token.ToJwtToken();
            if (jwt == null) return Guid.Empty;
            var clientId = jwt.Claims.FirstOrDefault(c => c.Type == "Gibson-ClientId")?.Value;
            return Guid.Parse(clientId);
        }

        private static JwtSecurityToken ToJwtToken(this string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;
            var jwt = handler.ReadJwtToken(token);
            return jwt;
        }
    }
}
