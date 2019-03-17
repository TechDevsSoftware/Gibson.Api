using System.Threading.Tasks;
using Gibson.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gibson.Common.Models;

namespace Gibson.Api.Controllers
{
    [Authorize]
    [Route("customer")]
    [ApiExplorerSettings(GroupName = "customer")]
    public class CustomerController : Controller
    {
        private readonly IUserService _users;
        public CustomerController(IUserService users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetCustomerProfile()
        {
            var clientId = this.ClientId();
            var userId = this.UserId();
            var user = await _users.FindById(userId, clientId);
            if (user == null) return new NotFoundResult();
            return new OkObjectResult(user?.UserProfile);
        }
    }
}