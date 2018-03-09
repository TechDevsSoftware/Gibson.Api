using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Core.UserManagement.Models;

namespace TechDevs.Core.UserManagement
{
    // API Schema 
    // /api/v1/{module}/{controller}/{method}
    [Route("api/v1/usermanagement/profiles")]
    public class UsersController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration registration)
        {
            try
            {
                if (registration == null) return new BadRequestObjectResult("Invalid registration");
                var result = await _userService.RegisterUser(registration);
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllUsers();
            return new OkObjectResult(result);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var result = await _userService.GetByEmail(email);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost("updateemail")]
        public async Task<IActionResult> UpdateEmail([FromQuery] string currentEmail, [FromQuery]string newEmail)
        {
            try
            {
                var result = await _userService.UpdateEmail(currentEmail, newEmail);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> Delete(string email)
        {
            try
            {
                await _userService.Delete(email);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost("password")]
        public async Task<IActionResult> SetPassword(string email, string password)
        {
            try
            {
                await _userService.SetPassword(email, password);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                var isValid = await _userService.ValidatePassword(loginRequest.Email, loginRequest.Password);
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