using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Routing.Matching;

namespace Gibson.Api
{
    public static class ExtMethods
    {
        public static Guid UserId(this Controller controller)
        {
            var user = controller.User;
            var val = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            return Guid.Parse(val);
        }

        public static Guid UserId(this ClaimsPrincipal user)
        {
            var val = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            return string.IsNullOrEmpty(val) ? Guid.Empty : Guid.Parse(val);
        }

        public static Guid ClientId(this ClaimsPrincipal user)
        {
            var val = user.FindFirstValue("Gibson-ClientId");
            return string.IsNullOrEmpty(val) ? Guid.Empty : Guid.Parse(val);
        }

        public static Guid ClientId(this Controller controller)
        {
            var user = controller.User;
            var val = user.FindFirst("Gibson-ClientId")?.Value;
            if (val != null) return Guid.Parse(val);
            controller.Request.Headers.TryGetValue("Gibson-ClientId", out var vals);
            val = vals.FirstOrDefault();
            return val == null ? Guid.Empty : Guid.Parse(val);
        }

        public static string GetClientKey(this Controller controller)
        {
            var user = controller.User;
            var val = user.FindFirst("Gibson-ClientKey")?.Value;
            return val;
        }

        public static string ClientKey(this HttpRequest request)
        {
            request.Headers.TryGetValue("Gibson-ClientKey", out var clientKey);
            return clientKey;
        }

        public static string GetClientKeyFromUri(this Uri uri)
        {
            if (string.IsNullOrEmpty(uri?.LocalPath) || uri?.LocalPath == "/") return null;

            var parts = uri.LocalPath.Split("/");
            var clientKey = parts[1];
            return clientKey;
        }

        public static string GetTokenFromRequest(this HttpRequest request)
        {
            var result = request.Headers["Authorization"].ToString();
            if(result.ToUpperInvariant().StartsWith("BEARER "))
            {
                result = result.Substring(7, result.Length - 7);
            }
            return result;
        }
        
        public static string GetAppId(this HttpRequest request)
        {
            return null;
        }
    }
}
