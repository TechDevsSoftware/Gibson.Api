using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/employees/account")]
    public class EmployeeRegistrationService : Controller
    {
        private readonly IAuthUserService<Employee> _userService;

        public EmployeeRegistrationService(IAuthUserService<Employee> userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] AuthUserRegistration registration, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            try
            {
                if (registration == null) return new BadRequestObjectResult("Invalid Registration");
                var result = await _userService.RegisterUser(registration, clientId);
                return new OkObjectResult(new EmployeeProfile(result));
            }
            catch (UserRegistrationException ex)
            {
                return new BadRequestObjectResult("Validation Errors: " + Environment.NewLine + ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("invite")]
        public async Task<IActionResult> InviteEmployee([FromBody] AuthUserInvitationRequest invite, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            try
            {
                if (clientId == null) return new BadRequestObjectResult("Invalid ClientId");
                if (invite == null) return new BadRequestObjectResult("Invalid invitation");
                var result = await _userService.SubmitInvitation(invite, clientId);
                return new OkObjectResult(new EmployeeProfile(result));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpGet]   
        [Route("invite/profile/{inviteKey}")]
        public async Task<IActionResult> GetUserProfileFromInviteKey(string inviteKey, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            var user = await _userService.GetUserByInviteKey(inviteKey, clientId);
            if (user == null) return new BadRequestObjectResult("User not found");
            return new OkObjectResult(new EmployeeProfile(user));
        }


        [HttpPost]
        [Route("invite/complete")]
        public async Task<IActionResult> CompleteInviteRegistration([FromBody] AuthUserInvitationAcceptRequest req, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            try
            {
                if (clientId == null) return new BadRequestObjectResult("Invalid ClientId");
                var result = await _userService.AcceptInvitation(req, clientId);
                return new OkObjectResult(new EmployeeProfile(result));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("invite/resend/{userId}")]
        public async Task<IActionResult> ResendInvitation(string email, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            await _userService.SendEmailInvitation(email, clientId);
            return new OkResult();
        }

    }
}