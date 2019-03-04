using System;
using System.Threading.Tasks;
using Gibson.Auth;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.Api.Controllers
{
    [Route("api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var token = await _authService.Login(loginRequest);
                return new OkObjectResult(token);
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
        }
    }
}