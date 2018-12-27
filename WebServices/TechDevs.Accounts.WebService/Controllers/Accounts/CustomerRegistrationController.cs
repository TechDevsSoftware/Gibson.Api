using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/customer/account/register")]
    public class CustomerRegistrationController : Controller
    {
        private readonly IAuthUserService<Customer> _userService;
        private readonly IClientService _clientService;

        public CustomerRegistrationController(IAuthUserService<Customer> userService, IClientService clientService)
        {
            _userService = userService;
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] AuthUserRegistration registration, [FromHeader(Name = "TechDevs-ClientKey")] string clientKey)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(clientKey);
                if (registration == null) return new BadRequestObjectResult("Invalid registration");
                var result = await _userService.RegisterUser(registration, client.Id);
                return new OkObjectResult(result);
            }
            catch (UserRegistrationException ex)
            {
                return new BadRequestObjectResult("Validaiton Errors: " + Environment.NewLine + ex.Message);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}