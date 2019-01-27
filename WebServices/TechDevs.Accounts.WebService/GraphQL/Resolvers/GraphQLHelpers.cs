using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TechDevs.Users.GraphQL.Resolvers
{
    public static class GraphQLHelpers
    {
        public static string GetClientKey(this IHttpContextAccessor httpContext)
        {
            httpContext.HttpContext.Request.Headers.TryGetValue("TechDevs-ClientKey", out var keys);
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
            var val = httpContext?.HttpContext?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            return val;
        }
    }
}
