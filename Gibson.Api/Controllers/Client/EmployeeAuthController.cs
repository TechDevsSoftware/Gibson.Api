using Gibson.Auth;
using Gibson.Common.Models;
using Gibson.Users;
using Microsoft.AspNetCore.Mvc;

namespace Gibson.Api.Controllers
{
    [Route("employee/auth")]
    [ApiExplorerSettings(GroupName = "client")]
    public class EmployeeAuthController : AuthController
    {
        public EmployeeAuthController(IAuthService authService, IUserRegistrationService userRegistrationService) 
            : base(authService, userRegistrationService, GibsonUserType.Employee)
        {
        }
    }
}