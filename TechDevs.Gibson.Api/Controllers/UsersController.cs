using System;
using System.Threading.Tasks;
using Gibson.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Shared.Models;

namespace TechDevs.Gibson.Api.Controllers
{
    [Route("api/v1/users")]
    public class UsersController : Controller
    {
        private readonly IUserService _users;
        private readonly IUserRegistrationService _regService;

        public UsersController(IUserService users, IUserRegistrationService regService)
        {
            _users = users;
            _regService = regService;
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUserById([FromRoute] Guid userId)
        {
            var clientId = this.ClientId();
            var result = await _users.FindById(userId, clientId);
            return new OkObjectResult(result);
        }

        [Authorize]
        [HttpGet("{userType}/{username}")]
        public async Task<ActionResult<User>> GetUserByUsername([FromRoute] GibsonUserType userType, [FromRoute] string username)
        {
            var clientId = this.ClientId();
            var result = await _users.FindByUsername(username, userType, clientId);
            return new OkObjectResult(result);
        }

        [Authorize]
        [HttpPut("{userId}")]
        public async Task<ActionResult<User>> UpdateUserProfile([FromRoute] Guid userId, [FromBody] UserProfile userProfile)
        {
            var clientId = this.ClientId();
            var result = await _users.UpdateUserProfile(userId, userProfile, clientId);
            return new OkObjectResult(result);
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var clientId = this.ClientId();
            await _users.DeleteUser(userId, clientId);
            return new OkResult();
        }

        [HttpPost]
        public async Task<ActionResult<User>> RegisterUser([FromBody] UserRegistration reg)
        {
            try
            {
                var clientId = this.ClientId();
                var result = await _regService.RegisterUser(reg, clientId);
                return new OkObjectResult(result);
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