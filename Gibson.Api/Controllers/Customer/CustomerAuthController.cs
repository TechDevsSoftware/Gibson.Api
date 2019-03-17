using Gibson.Auth;
using Gibson.Users;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;

namespace Gibson.Api.Controllers
{
    [Route("customer/auth")]
    [ApiExplorerSettings(GroupName = "customer")]
    public class CustomerAuthController : AuthController
    {
        public CustomerAuthController(IAuthService authService, IUserRegistrationService userRegistrationService) 
            : base(authService, userRegistrationService, GibsonUserType.Customer)
        {
        }
    }
}