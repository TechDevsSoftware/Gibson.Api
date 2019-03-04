using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using TechDevs.Shared.Utils;

namespace TechDevs.Gibson.Api
{
    public class ClientValidationMiddleware
    {
        private readonly RequestDelegate next;

        public ClientValidationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var header = context.Request.ClientKey();
            var referer = context.Request.GetTypedHeaders()?.Referer.GetClientKeyFromUri();
            var token = context.Request.GetTokenFromRequest()?.GetClientKey();

            var hasHeader = !string.IsNullOrEmpty(header);
            var hasReferer = !string.IsNullOrEmpty(referer);
            var hasToken = !string.IsNullOrEmpty(token);

            // Check for whitelisted keywords
            if (hasReferer)
            {
                bool ignore = false;
                if (referer == "ui") ignore = true;
                if (referer == "swagger") ignore = true;

                if (ignore)
                {
                    hasReferer = false;
                    referer = null;
                }

            }

            if (hasHeader && hasReferer && header != referer) throw new UnauthorizedAccessException();
            if (hasHeader && hasToken && header != token) throw new UnauthorizedAccessException();
            if (hasToken && hasReferer && token != referer) throw new UnauthorizedAccessException();

            if(!hasToken & !hasHeader & hasReferer)
            {
                context.Request.Headers.Add("Gibson-ClientKey", referer);
            }

            await next(context);
        }
    }
}
