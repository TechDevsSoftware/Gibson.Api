using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Accounts.Services;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Authorize]
    [Route("api/v1/customer/account")]
    public class CustomerController : Controller
    {
        private readonly IAuthUserService<Customer> _accountService;
        private readonly IClientService _clientService;

        public CustomerController(IAuthUserService<Customer> accountService, IClientService clientService)
        {
            _accountService = accountService;
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile([FromHeader(Name = "TechDevs-ClientKey")] string clientKey)
        {
            var client = await _clientService.GetClientByShortKey(clientKey); 
            var userId = GetUserIdFromRequest();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            return new OkObjectResult(new CustomerProfile(user));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount([FromHeader(Name = "TechDevs-ClientKey")] string clientKey)
        {
            var client = await _clientService.GetClientByShortKey(clientKey);

            var userId = GetUserIdFromRequest();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.Delete(user.EmailAddress, client.Id);
            if (result == false) return new BadRequestResult();
            return new OkResult();
        }

        private string GetUserIdFromRequest()
        {
            var sub = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (sub == null) sub = User.FindFirst("sub")?.Value;
            return sub;
        }
    }

    [Authorize]
    [Route("api/v1/employee/account")]
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

            var userId = GetUserIdFromRequest();
            if (userId == null) return new UnauthorizedResult();

            var emp = await _accountService.GetById(userId, client.Id);
            if (emp == null) return new NotFoundResult();

            return new OkObjectResult(new EmployeeProfile(emp));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount([FromHeader(Name = "TechDevs-ClientId")] string clientKey)
        {
            var client = await _clientService.GetClientByShortKey(clientKey);

            var userId = GetUserIdFromRequest();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.Delete(user.EmailAddress, client.Id);
            if (result == false) return new BadRequestResult();
            return new OkResult();
        }

        private string GetUserIdFromRequest()
        {
            var sub = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (sub == null) sub = User.FindFirst("sub")?.Value;
            return sub;
        }
    }
}