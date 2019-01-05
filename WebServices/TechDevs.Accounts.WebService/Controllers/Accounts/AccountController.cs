using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Route("api/v1/customers")]
    public class CustomerController : AccountController<Customer>
    {
        private readonly IAuthUserService<Customer> _accountService;
        private readonly IClientService _clientService;

        public CustomerController(IAuthTokenService<Customer> tokenService, IAuthUserService<Customer> accountService, IClientService clientService)
            : base(tokenService, accountService, clientService)
        {
            _accountService = accountService;
            _clientService = clientService;
        }


        [HttpGet]
        [Produces(typeof(CustomerProfile))]
        public override async Task<IActionResult> GetProfile()
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            return new OkObjectResult(new CustomerProfile(user));
        }
    }

    [Route("api/v1/employees")]
    public class EmployeeController : AccountController<Employee>
    {
        public EmployeeController(IAuthTokenService<Employee> tokenService, IAuthUserService<Employee> accountService, IClientService clientService)
            : base(tokenService, accountService, clientService)
        {
        }
    }

    [Authorize]
    public abstract class AccountController<TAuthUser> : AuthController<TAuthUser> where TAuthUser : AuthUser, new()
    {
        private readonly IAuthUserService<TAuthUser> _accountService;
        private readonly IClientService _clientService;

        protected AccountController(IAuthTokenService<TAuthUser> tokenService, IAuthUserService<TAuthUser> accountService, IClientService clientService)
            : base(tokenService, accountService, clientService)
        {
            _accountService = accountService;
            _clientService = clientService;
        }

        [HttpGet]
        [Produces(typeof(AuthUserProfile))]
        public virtual async Task<IActionResult> GetProfile()
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            return new OkObjectResult(new AuthUserProfile(user));
        }

        [HttpDelete]
        [Produces(typeof(void))]
        public async Task<IActionResult> DeleteAccount()
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());

            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.Delete(user.EmailAddress, client.Id);
            if (result == false) return new BadRequestResult();
            return new OkResult();
        }

        [HttpPost]
        [Produces(typeof(AuthUserProfile))]
        [Route("invite")]
        public async Task<IActionResult> InviteEmployee([FromBody] AuthUserInvitationRequest invite)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
                if (invite == null) return new BadRequestObjectResult("Invalid invitation");
                var result = await _accountService.SubmitInvitation(invite, client.Id);
                return new OkObjectResult(new AuthUserProfile(result));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpGet]
        [Produces(typeof(AuthUserProfile))]
        [Route("invite/profile/{inviteKey}")]
        public async Task<IActionResult> GetUserProfileFromInviteKey(string inviteKey)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var user = await _accountService.GetUserByInviteKey(inviteKey, client.Id);
            if (user == null) return new BadRequestObjectResult("User not found");
            return new OkObjectResult(new AuthUserProfile(user));
        }

        [HttpPost]
        [Produces(typeof(AuthUserProfile))]
        [Route("invite/complete")]
        public async Task<IActionResult> CompleteInviteRegistration([FromBody] AuthUserInvitationAcceptRequest request)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
                var result = await _accountService.AcceptInvitation(request, client.Id);
                return new OkObjectResult(new AuthUserProfile(result));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost]
        [Produces(typeof(void))]
        [Route("invite/resend/{userId}")]
        public async Task<IActionResult> ResendInvitation(string email)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            await _accountService.SendEmailInvitation(email, client.Id);
            return new OkResult();
        }

        [HttpPost("setname")]
        [Produces(typeof(AuthUserProfile))]
        public async Task<IActionResult> UpdateAccountName([FromQuery] string firstName, [FromQuery] string lastName)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.UpdateName(user.EmailAddress, firstName, lastName, client.Id);

            return new OkObjectResult(new AuthUserProfile(result));
        }

        [HttpPost("setcontactdetails")]
        [Produces(typeof(AuthUserProfile))]
        public async Task<IActionResult> UpdateAccountContactDetails([FromQuery]string contactNumber)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.UpdateContactNuber(user.EmailAddress, contactNumber, client.Id);

            return new OkObjectResult(new AuthUserProfile(result));
        }

        [HttpPost]
        [Route("register")]
        [Produces(typeof(AuthUserProfile))]
        public async Task<IActionResult> RegisterUser([FromBody] AuthUserRegistration registration)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
                if (registration == null) return new BadRequestObjectResult("Invalid Registration");
                var result = await _accountService.RegisterUser(registration, client.Id);
                return new OkObjectResult(new AuthUserProfile(result));
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
    }
}