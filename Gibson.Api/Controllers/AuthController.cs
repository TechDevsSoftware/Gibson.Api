using System;
using System.Threading.Tasks;
using Gibson.Auth;
using Gibson.Common.Enums;
using Gibson.Common.Models;
using Gibson.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Gibson.Api.Controllers
{
    public abstract class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly GibsonUserType _userType;

        protected AuthController(IAuthService authService, IUserRegistrationService userRegistrationService,
            GibsonUserType userType)
        {
            _authService = authService;
            _userRegistrationService = userRegistrationService;
            _userType = userType;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest loginRequest, [FromRoute] Guid clientId)
        {
            try
            {
                var token = await _authService.Login(loginRequest, _userType, clientId);
                return new OkObjectResult(token);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<PublicUser>> RegisterUser([FromBody] UserRegistration reg, [FromRoute] Guid clientId)
        {
            try
            {
                reg.UserType = _userType;
                var result = await _userRegistrationService.RegisterUser(reg, clientId);
                return new OkObjectResult(new PublicUser(result));
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