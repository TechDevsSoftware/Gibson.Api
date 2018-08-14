using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthTokenService _tokenService;
        private readonly IAccountService _accountService;

        public AuthController(IAuthTokenService tokenService, IAccountService accountService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            switch (req.Provider)
            {
                case "TechDevs":
                    return await LoginWithTechDevs(req.Email, req.Password);
                case "Google":
                    return await LoginWithGoogle(req.ProviderIdToken);
                default:
                    return new BadRequestObjectResult("Unsupported auth provider");
            }
        }

        private async Task<IActionResult> LoginWithTechDevs(string email, string password)
        {
            var valid = await _accountService.ValidatePassword(email, password);
            if (!valid) return new UnauthorizedResult();
            var user = await _accountService.GetByEmail(email);
            var token = await _tokenService.CreateToken(user.Id, "profile");
            return new OkObjectResult(token);
        }

        private async Task<IActionResult> LoginWithGoogle(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                var user = await _accountService.GetByProvider("Google", payload.Subject);
                var token = await _tokenService.CreateToken(user.Id, "profile");
                return new OkObjectResult(token);
            }
            catch (InvalidJwtException ex)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new UnauthorizedResult();
            }
        }
    }
}