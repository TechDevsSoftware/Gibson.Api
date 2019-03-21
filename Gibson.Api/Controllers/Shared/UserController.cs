using System.Threading.Tasks;
using Gibson.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;

namespace Gibson.Api.Controllers
{
    [Authorize]
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService _users;
        public UserController(IUserService users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetUserProfile()
        {
            var clientId = this.ClientId();
            var userId = this.UserId();
            var user = await _users.FindById(userId, clientId);
            if (user == null) return new NotFoundResult();
            return new OkObjectResult(user?.UserProfile);
        }
    }
}