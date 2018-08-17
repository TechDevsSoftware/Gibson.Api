﻿using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Authorize]
    [Route("api/v1/account")]
    public class CustomerController : Controller
    {
        private readonly IAuthUserService<Customer> _accountService;

        public CustomerController(IAuthUserService<Customer> accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile([FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            var userId = GetUserIdFromRequest();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, clientId);
            if (user == null) return new NotFoundResult();

            return new OkObjectResult(new CustomerProfile(user));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount([FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            var userId = GetUserIdFromRequest();
            if (userId == null) return new UnauthorizedResult();

            var user = await _accountService.GetById(userId, clientId);
            if (user == null) return new NotFoundResult();

            var result = await _accountService.Delete(user.EmailAddress, clientId);
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