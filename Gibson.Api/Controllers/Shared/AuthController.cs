using System;
using System.Threading.Tasks;
using Gibson.Auth;
using Gibson.Common.Models;
using Gibson.Users;
using Microsoft.AspNetCore.Mvc;

namespace Gibson.Api.Controllers
{
    public abstract class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly GibsonUserType _userType;
        
        protected AuthController(IAuthService authService, IUserRegistrationService userRegistrationService, GibsonUserType userType)
        {
            _authService = authService;
            _userRegistrationService = userRegistrationService;
            _userType = userType;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var clientId = this.ClientId();
                var token = await _authService.Login(loginRequest, _userType, clientId);
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