using System;
using System.Linq;
using System.Threading.Tasks;
using Gibson.Common.Enums;
using Gibson.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;

namespace Gibson.Api.Controllers
{
    [Route("clients/{clientId}")]
    public class UsersController : Controller
    {
        private readonly IUserService _users;

        public UsersController(IUserService users)
        {
            _users = users;
        }

        [HttpGet("users/{userId}")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult<UserProfile>> GetUserProfile([FromRoute]Guid userId, [FromRoute] Guid clientId)
        {
            var user = await _users.FindById(userId, clientId);
            if (user == null) return new NotFoundResult();
            return new OkObjectResult(user?.UserProfile);
        }

        [HttpGet("employees")]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<UserProfile>> GetClientEmployees([FromRoute] Guid clientId)
        {
            var employees = await _users.FindByClient(GibsonUserType.ClientEmployee, clientId);
            if (employees == null) return new NotFoundResult();
            return new OkObjectResult(employees.Select(x => x.UserProfile));
        }

        [HttpGet("customers")]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<UserProfile>> GetClientCustomers([FromRoute] Guid clientId)
        {
            var customers = await _users.FindByClient(GibsonUserType.Customer, clientId);
            if (customers == null) return new NotFoundResult();
            return new OkObjectResult(customers.Select(x => x.UserProfile));
        }

        [HttpDelete("users/{userId}")]
        [Authorize(Policy = "CustomerData")]
        public async Task<ActionResult> DeleteUser([FromRoute] Guid userId, [FromRoute] Guid clientId)
        {
            try
            {
                await _users.DeleteUser(userId, clientId);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }
    }
}