using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Clients;
using TechDevs.Shared.Models;
using TechDevs.MarketingPreferences;
using TechDevs.NotificationPreferences;
using TechDevs.Customers;

namespace TechDevs.Gibson.WebService.Controllers
{
    [Route("api/v1/customer/account")]
    public class CustomerDataController : Controller
    {
        private readonly IClientService _clientService;
        private readonly INotificationPreferencesService _notificationPreferences;
        private readonly IMarketingPreferencesService _marketingService;
        private readonly ICustomerService _customerService;

        public CustomerDataController(
            IClientService clientService,
            IMarketingPreferencesService marketingService,
            INotificationPreferencesService notificationPreferences,
            ICustomerService customerService
            )
        {
            _clientService = clientService;
            _notificationPreferences = notificationPreferences;
            _marketingService = marketingService;
            _customerService = customerService;
        }

        [HttpGet]
        [Produces(typeof(Customer))]
        public async Task<IActionResult> GetCustomerData()
        {
            var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
            return new OkObjectResult(await _customerService.GetById(this.UserId(), client.Id));
        }

        [HttpPost("preferences/marketing")]
        [Produces(typeof(Customer))]
        public async Task<IActionResult> UpdateMarketingPreferences([FromBody] MarketingNotificationPreferences marketingPreferences)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
                var userId = this.UserId();
                if (userId == null) return new UnauthorizedResult();

                var result = await _marketingService.UpdateMarketingPreferences(marketingPreferences, userId, client.Id);
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Marketing preferences could not be updated");
            }
        }

        [HttpPost("preferences/notifications")]
        [Produces(typeof(Customer))]
        public async Task<IActionResult> UpdateNotificationPreferences([FromBody] CustomerNotificationPreferences notificationPreferences)
        {
            try
            {
                var client = await _clientService.GetClientByShortKey(Request.GetClientKey());
                var userId = this.UserId();
                if (userId == null) return new UnauthorizedResult();

                var result = await _notificationPreferences.UpdateNotificationPreferences(notificationPreferences, userId, client.Id);
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Notification preferences could not be updated");
            }
        }
    }
}