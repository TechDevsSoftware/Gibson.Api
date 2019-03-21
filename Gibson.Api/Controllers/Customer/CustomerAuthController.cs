using Gibson.Auth;
using Gibson.Common.Enums;
using Gibson.Users;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace Gibson.Api.Controllers
{
    [Route("clients/{clientId}/customer/auth")]
    [ApiExplorerSettings(GroupName = "customer")]
    public class CustomerAuthController : AuthController
    {
        public CustomerAuthController(IAuthService authService, IUserRegistrationService userRegistrationService) 
            : base(authService, userRegistrationService, GibsonUserType.Customer)
        {
        }
    }
}