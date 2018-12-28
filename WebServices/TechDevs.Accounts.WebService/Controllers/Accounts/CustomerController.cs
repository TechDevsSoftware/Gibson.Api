using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Accounts;
using TechDevs.Accounts.WebService;
using TechDevs.Clients;
using TechDevs.Shared.Models;

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
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            return new OkObjectResult(new CustomerProfile(user));
        }

        [HttpPost("setname")]
        public async Task<IActionResult> UpdateAccountName([FromHeader(Name = "TechDevs-ClientKey")] string clientKey, [FromQuery] string firstName, [FromQuery] string lastName)
        {
            var client = await _clientService.GetClientByShortKey(clientKey);
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.UpdateName(user.EmailAddress, firstName, lastName, client.Id);

            return new OkObjectResult(new CustomerProfile(result));
        }

        [HttpPost("setcontactdetails")]
        public async Task<IActionResult> UpdateAccountContactDetails([FromHeader(Name = "TechDevs-ClientKey")] string clientKey, [FromQuery]string contactNumber)
        {
            var client = await _clientService.GetClientByShortKey(clientKey);
            var userId = this.UserId();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, client.Id);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.UpdateContactNuber(user.EmailAddress, contactNumber, client.Id);

            return new OkObjectResult(new CustomerProfile(result));
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
    }
}