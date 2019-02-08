using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TechDevs.Users.GraphQL.Resolvers
{
    public static class GraphQLHelpers
    {
        public static string GetClientKey(this IHttpContextAccessor httpContext)
        {
            httpContext.HttpContext.Request.Headers.TryGetValue("Gibson-ClientKey", out var keys);
            var clientKey = keys.FirstOrDefault();
            if (string.IsNullOrEmpty(clientKey)) return null;
            return clientKey;
        }

        public static string GetAuthToken(this IHttpContextAccessor httpContext)
        {
            httpContext.HttpContext.Request.Headers.TryGetValue("Authorization", out var keys);
            var clientKey = keys.FirstOrDefault();
            if (string.IsNullOrEmpty(clientKey)) return null;
            return clientKey.Replace("Bearer ", "");
        }

        public static string GetUserId(this IHttpContextAccessor httpContext)
        {
            var token = GetAuthToken(httpContext);

            if (token == null) throw new Exception("Token missing. Cannot authenticate user");

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) throw new Exception("Jwt token cannot be read");
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            return userId;

        }
    }
}
