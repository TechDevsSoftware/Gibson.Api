using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<PublicUser>> GetUserProfile([FromRoute]Guid userId, [FromRoute] Guid clientId)
        {
            var user = await _users.FindById(userId, clientId);
            if (user == null) return new NotFoundResult();
            return new OkObjectResult(new PublicUser(user));
        }

        [HttpGet("employees")]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<List<PublicUser>>> GetClientEmployees([FromRoute] Guid clientId)
        {
            try
            {
                var employees = await _users.FindByClient(GibsonUserType.ClientEmployee, clientId);
                if (employees == null) return new NotFoundResult();
                return new OkObjectResult(employees.Select(x => new PublicUser(x)));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [HttpGet("customers")]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<List<PublicUser>>> GetClientCustomers([FromRoute] Guid clientId)
        {
            try
            {
                var customers = await _users.FindByClient(GibsonUserType.Customer, clientId);
                if (customers == null) return new NotFoundResult();
                return new OkObjectResult(customers.Select(x => new PublicUser(x)));
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
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

        [HttpPut("users/{userId}")]
        [Authorize(Policy = "ClientData")]
        public async Task<ActionResult<PublicUser>> UpdateUserProfile([FromRoute] Guid clientId, [FromRoute] Guid userId, [FromBody] UserProfile userProfile)
        {
            try
            {
                var updateResult = await _users.UpdateUserProfile(userId, userProfile, clientId);
                return new OkObjectResult(new PublicUser(updateResult));
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }
    }
}