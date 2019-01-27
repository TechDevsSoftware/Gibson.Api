using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Customers;
using TechDevs.Employees;
using TechDevs.Shared.Models;
using TechDevs.Users;

namespace TechDevs.Gibson.Api.Controllers
{
    [Route("api/v1/customers")]
    public class CustomerController : AccountController<Customer>
    {
        private readonly ICustomerService _customerService;
        private readonly IClientService _clientService;

        public CustomerController(IAuthTokenService<Customer> tokenService, ICustomerService customerService, IClientService clientService, IAuthService<Customer> auth)
            : base(tokenService, customerService, clientService, auth)
        {
            _customerService = customerService;
            _clientService = clientService;
        }


        [HttpGet]
        [Produces(typeof(CustomerProfile))]
        public override async Task<IActionResult> GetProfile()
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _customerService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            return new OkObjectResult(new CustomerProfile(user));
        }
    }

    [Route("api/v1/employees")]
    public class EmployeeController : AccountController<Employee>
    {
        private readonly IEmployeeService _employeeService;
        private readonly IClientService _clientService;

        public EmployeeController(IAuthTokenService<Employee> tokenService,IEmployeeService employeeService, IClientService clientService, IAuthService<Employee> auth)
            : base(tokenService, employeeService, clientService, auth)
        {
            _employeeService = employeeService;
            _clientService = clientService;
        }


        [HttpGet]
        [Produces(typeof(EmployeeProfile))]
        public override async Task<IActionResult> GetProfile()
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _employeeService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            return new OkObjectResult(new EmployeeProfile(user));
        }
    }

    [Authorize]
    public abstract class AccountController<TAuthUser> : AuthController<TAuthUser> where TAuthUser : AuthUser, new()
    {
        private readonly IUserService<TAuthUser> _userService;
        private readonly IClientService _clientService;

        protected AccountController(IAuthTokenService<TAuthUser> tokenService, IUserService<TAuthUser> accountService, IClientService clientService, IAuthService<TAuthUser> auth)
            : base(tokenService, accountService, clientService, auth)
        {
            _userService = accountService;
            _clientService = clientService;
        }

        [HttpGet]
        [Produces(typeof(AuthUserProfile))]
        public virtual async Task<IActionResult> GetProfile()
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _userService.GetById(userId, client.Id);
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

            var user = await _userService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _userService.Delete(user.EmailAddress, client.Id);
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
                var result = await _userService.SubmitInvitation(invite, client.Id);
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
            var user = await _userService.GetUserByInviteKey(inviteKey, client.Id);
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
                var result = await _userService.AcceptInvitation(request, client.Id);
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
            await _userService.SendEmailInvitation(email, client.Id);
            return new OkResult();
        }

        [HttpPost("setname")]
        [Produces(typeof(AuthUserProfile))]
        public async Task<IActionResult> UpdateAccountName([FromQuery] string firstName, [FromQuery] string lastName)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _userService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _userService.UpdateName(user.EmailAddress, firstName, lastName, client.Id);

            return new OkObjectResult(new AuthUserProfile(result));
        }

        [HttpPost("setcontactdetails")]
        [Produces(typeof(AuthUserProfile))]
        public async Task<IActionResult> UpdateAccountContactDetails([FromQuery]string contactNumber)
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _userService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _userService.UpdateContactNuber(user.EmailAddress, contactNumber, client.Id);

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
                var result = await _userService.RegisterUser(registration, client.Id);
                return new OkObjectResult(new AuthUserProfile(result));
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