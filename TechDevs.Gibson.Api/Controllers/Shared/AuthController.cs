using System;
using System.Threading.Tasks;
using Gibson.Auth;
using Gibson.Users;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.Api.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserRegistrationService _userRegistrationService;

        public AuthController(IAuthService authService, IUserRegistrationService userRegistrationService)
        {
            _authService = authService;
            _userRegistrationService = userRegistrationService;
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
        
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody] UserRegistration reg)
        {
            try
            {
                var clientId = this.ClientId();
                var result = await _userRegistrationService.RegisterUser(reg, clientId);
                return new OkObjectResult(result);
            }
            catch (UserRegistrationException ex)
            {
                return new BadRequestObjectResult(ex.RegistrationErrors);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}