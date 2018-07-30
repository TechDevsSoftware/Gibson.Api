using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var user = await _accountService.GetByEmail(req.Email);
            var token = await _tokenService.CreateToken(user.Id, "profile");
            return new OkObjectResult(token);
        }
    }
}