using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TechDevs.Gibson.WebService
{
    public static class ExtMethods
    {
        public static string UserId(this Controller controller)
        {
            var user = controller.User;
            var val = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            return val;
        }

        public static string GetClientKey(this HttpRequest request)
        {
            request.Headers.TryGetValue("TechDevs-ClientKey", out var clientKey);
            if (string.IsNullOrEmpty(clientKey))  throw new Exception("Client Key not found"); 
            return clientKey;
        }
    }
}
