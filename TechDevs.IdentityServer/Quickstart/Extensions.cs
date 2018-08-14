using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using TechDevs.Accounts;

namespace IdentityServer4.Quickstart.UI
{
    public class PasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IAccountService _accountService;

        public PasswordValidator(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var result = await _accountService.ValidatePassword(context.UserName, context.Password);
            if (result)
            {
                var user = await _accountService.GetByEmail(context.UserName);
                if (user != null)
                {
                    context.Result = new GrantValidationResult(subject: user.Id, authenticationMethod: "custom");
                }
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
            }
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Determines whether the client is configured to use PKCE.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="client_id">The client identifier.</param>
        /// <returns></returns>
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string client_id)
        {
            if (!string.IsNullOrWhiteSpace(client_id))
            {
                var client = await store.FindEnabledClientByIdAsync(client_id);
                return client?.RequirePkce == true;
            }

            return false;
        }
    }
}
