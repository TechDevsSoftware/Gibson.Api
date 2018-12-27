using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService
{
    // API Schema
    [Route("api/v1/admin/accounts")]
    //[Authorize]
    public class CustomerAdminController : ControllerBase
    {
        private readonly IAuthUserService<Customer> _userService;

        public CustomerAdminController(IAuthUserService<Customer> userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            var result = await _userService.GetAllUsers(clientId);
            return new OkObjectResult(result);
        }

        [HttpPost("updateemail")]
        public async Task<IActionResult> UpdateEmail([FromQuery] string currentEmail, [FromQuery]string newEmail, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            try
            {
                var result = await _userService.UpdateEmail(currentEmail, newEmail, clientId);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            try
            {
                await _userService.Delete(email, clientId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost("password")]
        public async Task<IActionResult> SetPassword(string email, string password, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            try
            {
                await _userService.SetPassword(email, password, clientId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest, [FromHeader(Name = "TechDevs-ClientId")] string clientId)
        {
            try
            {
                var isValid = await _userService.ValidatePassword(loginRequest.Email, loginRequest.Password, clientId);
                if (isValid) return new OkResult();
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}