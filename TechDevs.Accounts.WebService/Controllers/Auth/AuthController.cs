using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Accounts.Services;

namespace TechDevs.Accounts.WebService.Controllers
{
    public abstract class AuthController<TAuthUser> : Controller where TAuthUser : AuthUser, new()
    {
        private readonly IAuthTokenService<TAuthUser> _tokenService;
        private readonly IAuthUserService<TAuthUser> _accountService;
        private readonly IClientService _clientService;
        
        protected AuthController(IAuthTokenService<TAuthUser> tokenService, IAuthUserService<TAuthUser> accountService, IClientService clientService)
        {
            _tokenService = tokenService;
            _accountService = accountService;
            _clientService = clientService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, [FromHeader(Name = "TechDevs-ClientKey")] string clientKey)
        {
            // Get the clientId from the clientKey
            var client = await _clientService.GetClientByShortKey(clientKey); 
            switch (req.Provider)
            {
                case "TechDevs":
                    return await LoginWithTechDevs(req.Email, req.Password, client.Id);
                case "Google":
                    return await LoginWithGoogle(req.ProviderIdToken, client.Id);
                default:
                    return new BadRequestObjectResult("Unsupported auth provider");
            }
        }

        private async Task<IActionResult> LoginWithTechDevs(string email, string password, string clientId)
        {
            var valid = await _accountService.ValidatePassword(email, password, clientId);
            if (!valid) return new UnauthorizedResult();
            var user = await _accountService.GetByEmail(email, clientId);
            var token = await _tokenService.CreateToken(user.Id, "profile", clientId);
            return new OkObjectResult(token);
        }

        private async Task<IActionResult> LoginWithGoogle(string idToken, string clientId)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                var user = await _accountService.GetByProvider("Google", payload.Subject, clientId);

                if(user == null)
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
                    var regResult = await _accountService.RegisterUser(regRequest, clientId);
                    user = regResult;
                } 

                var token = await _tokenService.CreateToken(user.Id, "profile", clientId);
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

    [Produces("application/json")]
    [Route("api/v1/employee/auth")]
    public class EmployeAuthController : AuthController<Employee>
    {
        public EmployeAuthController(IAuthTokenService<Employee> tokenService, IAuthUserService<Employee> accountService, IClientService clientService) 
            : base(tokenService, accountService, clientService)
        {
        }
    }

    [Produces("application/json")]
    [Route("api/v1/customer/auth")]
    public class CustomerAuthController : AuthController<Customer>
    {
        public CustomerAuthController(IAuthTokenService<Customer> tokenService, IAuthUserService<Customer> accountService, IClientService clientService)
            : base(tokenService, accountService, clientService)
        {
        }
    }
}