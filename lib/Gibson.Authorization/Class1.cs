using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Authorization
{
    public class AuthValidationContext
    {
        public ClaimsPrincipal User { get; set; }
        public object Entity { get; set; }
    }

    public interface IAuthorizationValidator
    {
        Task<bool> Validate<T>(ClaimsPrincipal user, object entity);
    }

    public class CustomerEntityAuthorizationValidator<T> : IAuthorizationValidator where T : IEntity
    {
        public Task<bool> Validate<TEntity>(ClaimsPrincipal user, object entity)
        {
        }
    }
}