using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Core.UserManagement.Interfaces;
using TechDevs.Core.UserManagement.Models;

namespace TechDevs.Core.UserManagement.API.Controllers
{
    [Route("api/users")]
    public class UsersController
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IUserRegistrationService _userRegistrationService;

        public UsersController(IUserProfileService userProfileService, IUserRegistrationService userRegistrationService)
        {
            _userProfileService = userProfileService;
            _userRegistrationService = userRegistrationService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(string userId)
        {
            try
            {
                var result = await _userProfileService.GetUserProfile(userId);
                return new ObjectResult(result);
            }
            catch (System.Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration registration)
        {
            try
            {
                if (registration == null) return new BadRequestResult();
                var result = await _userRegistrationService.RegisterUser(registration);
                return new ObjectResult(result);
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