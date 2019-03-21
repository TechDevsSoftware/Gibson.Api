using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Gibson.Auth.Tokens;

namespace Gibson.Api
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
                var ignore = referer == "ui" || referer == "swagger";

                if (ignore)
                {
                    hasReferer = false;
                    referer = null;
                }
            }

            try
            {
                if (hasHeader && hasReferer && header != referer) throw new UnauthorizedAccessException();
                if (hasHeader && hasToken && header != token) throw new UnauthorizedAccessException();
                if (hasToken && hasReferer && token != referer) throw new UnauthorizedAccessException();
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }


            if (!hasToken & !hasHeader & hasReferer)
            {
                context.Request.Headers.Add("Gibson-ClientKey", referer);
            }

            await next(context);
        }
    }
}