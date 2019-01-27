using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Users.GraphQL.Resolvers
{
    public interface IResolver<T>
    {
    }

    [Authorize]
    public class ClientResolver
    {
        private readonly IClientService clientService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IAuthService<AuthUser> auth;

        public ClientResolver(IClientService clientService, IHttpContextAccessor httpContext, IAuthService<AuthUser> auth)
        {
            this.clientService = clientService;
            this.httpContext = httpContext;
            this.auth = auth;
        }

        public async Task<List<Client>> FindAll()
        {
            var result = await clientService.GetClients();
            return result;
        }

        public async Task<Client> FindOne(string id)
        {
            var token = httpContext.GetAuthToken();
            var userId = httpContext.GetUserId();

            var result = await clientService.GetClient(id);
            return result;
        }

        public async Task<Client> FindOneByKey(string key)
        {
            var result = await clientService.GetClientByShortKey(key);
            return result;
        }

        private bool IsAuthenticated()
        {
            var token = httpContext.GetAuthToken();
            var clientKey = httpContext.GetClientKey();
            return auth.ValidateToken(httpContext.GetAuthToken(), httpContext.GetClientKey());
        }
    }
}
