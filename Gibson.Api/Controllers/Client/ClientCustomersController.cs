using System;
using System.Threading.Tasks;
using Gibson.Common.Enums;
using Gibson.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;

namespace Gibson.Api.Controllers
{
    [Authorize]
    [Route("client/customers")]
    [ApiExplorerSettings(GroupName = "client")]
    public class ClientCustomersController : Controller
    {
        private readonly IUserService _users;
        private const GibsonUserType USER_TYPE = GibsonUserType.Customer;

        public ClientCustomersController(IUserService users)
        {
            _users = users;
        }
        
        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetCustomerById([FromRoute] Guid userId)
        {
            var clientId = this.ClientId();
            var result = await _users.FindById(userId, clientId);
            return new OkObjectResult(result);
        }

        [HttpGet("{userType}/{username}")]
        public async Task<ActionResult<User>> GetCustomerByUsername([FromRoute] string username)
        {
            var clientId = this.ClientId();
            var result = await _users.FindByUsername(username, USER_TYPE, clientId);
            return new OkObjectResult(result);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<User>> UpdateCustomerProfile([FromRoute] Guid userId, [FromBody] UserProfile userProfile)
        {
            var clientId = this.ClientId();
            var result = await _users.UpdateUserProfile(userId, userProfile, clientId);
            return new OkObjectResult(result);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteCustomer([FromRoute] Guid userId)
        {
            var clientId = this.ClientId();
            await _users.DeleteUser(userId, clientId);
            return new OkResult();
        }
    }
}