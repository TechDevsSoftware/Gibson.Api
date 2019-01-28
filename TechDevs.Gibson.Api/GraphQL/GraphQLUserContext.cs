using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TechDevs.Gibson.Api
{

    public class GraphQLUserContext
    {
        public ClaimsPrincipal User { get; set; }
        public IHeaderDictionary Headers { get; set; }
    }
}
