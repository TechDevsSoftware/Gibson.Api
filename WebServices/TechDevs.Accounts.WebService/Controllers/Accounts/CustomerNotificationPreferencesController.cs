using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.NotificationPreferences;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Route("api/v1/customer/account/notificationpreferences")]
    public class CustomerNotificationPreferencesController : Controller
    {
        private readonly IClientService _clientService;
        private readonly INotificationPreferencesService _notificationPreferences;

        public CustomerNotificationPreferencesController(IClientService clientService, INotificationPreferencesService notificationPreferences)
        {
            _clientService = clientService;
            _notificationPreferences = notificationPreferences;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNotificationPreferences([FromBody] CustomerNotificationPreferences notificationPreferences, [FromHeader(Name = "TechDevs-ClientKey")] string clientKey)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(clientKey);
                var userId = this.UserId();
                if (userId == null) return new UnauthorizedResult();

                 var result = await _notificationPreferences.UpdateNotificationPreferences(notificationPreferences, userId, client.Id);
                 return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Vehicle could not be added");
            }
        }
    }
}