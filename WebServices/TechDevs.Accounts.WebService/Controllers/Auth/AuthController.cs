using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;
using TechDevs.Users;

namespace TechDevs.Gibson.Api.Controllers
{
    [AllowAnonymous]
    public abstract class AuthController<TAuthUser> : Controller where TAuthUser : AuthUser, new()
    {
        private readonly IAuthTokenService<TAuthUser> _tokenService;
        private readonly IUserService<TAuthUser> _accountService;
        private readonly IClientService _clientService;
        private readonly IAuthService<TAuthUser> auth;

        protected AuthController(IAuthTokenService<TAuthUser> tokenService, IUserService<TAuthUser> accountService, IClientService clientService, IAuthService<TAuthUser> auth)
        {
            _tokenService = tokenService;
            _accountService = accountService;
            _clientService = clientService;
            this.auth = auth;
        }

        [HttpPost]
        [Route("login")]
        [Produces(typeof(string))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Get the clientId from the clientKey
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            switch (request.Provider)
            {
                case "TechDevs":
                    return await LoginWithTechDevs(request.Email, request.Password, client);
                case "Google":
                    return await LoginWithGoogle(request.ProviderIdToken, client);
                default:
                    return new BadRequestObjectResult("Unsupported auth provider");
            }
        }

        private async Task<IActionResult> LoginWithTechDevs(string email, string password, Client client)
        {
            var valid = await auth.ValidatePassword(email, password, client.Id);
            if (!valid) return new UnauthorizedResult();
            var user = await _accountService.GetByEmail(email, client.Id);
            var token = _tokenService.CreateToken(user.Id, client.ShortKey);
            return new OkObjectResult(token);
        }

        private async Task<IActionResult> LoginWithGoogle(string idToken, Client client)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                var user = await _accountService.GetByProvider("Google", payload.Subject, client.Id);

                if (user == null)
                {
                    var regRequest = new AuthUserRegistration
                    {
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        EmailAddress = payload.Email,
                        AggreedToTerms = true,
                        ProviderName = "Google",
                        ProviderId = payload.Subject,
                        Password = null,
                        ChangePasswordOnFirstLogin = false,
                        IsInvite = false
                    };
                    var regResult = await _accountService.RegisterUser(regRequest, client.Id);
                    user = regResult;
                }

                var token = _tokenService.CreateToken(user.Id, client.ShortKey);
                return new OkObjectResult(token);
            }
            catch (InvalidJwtException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new UnauthorizedResult();
            }
        }
    }
}