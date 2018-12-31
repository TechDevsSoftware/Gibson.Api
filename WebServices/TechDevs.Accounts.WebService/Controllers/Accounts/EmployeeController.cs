using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Authorize]
    [Route("api/v1/employees/account")]
    public class EmployeeController : Controller
    {
        private readonly IAuthUserService<Employee> _accountService;
        private readonly IClientService _clientService;

        public EmployeeController(IAuthUserService<Employee> accountService, IClientService clientService)
        {
            _accountService = accountService;
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile([FromHeader(Name = "TechDevs-ClientKey")] string clientKey)
        {
            var client = await _clientService.GetClientByShortKey(clientKey);

            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var emp = await _accountService.GetById(userId, client.Id);
            if (emp == null) return new NotFoundResult();

            return new OkObjectResult(new EmployeeProfile(emp));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount([FromHeader(Name = "TechDevs-ClientKey")] string clientKey)
        {
            var client = await _clientService.GetClientByShortKey(clientKey);

            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.Delete(user.EmailAddress, client.Id);
            if (result == false) return new BadRequestResult();
            return new OkResult();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] AuthUserRegistration registration, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            try
            {
                if (registration == null) return new BadRequestObjectResult("Invalid Registration");
                var result = await _accountService.RegisterUser(registration, clientId);
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
                var result = await _accountService.SubmitInvitation(invite, clientId);
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
            var user = await _accountService.GetUserByInviteKey(inviteKey, clientId);
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
                var result = await _accountService.AcceptInvitation(req, clientId);
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
            await _accountService.SendEmailInvitation(email, clientId);
            return new OkResult();
        }

    }
}