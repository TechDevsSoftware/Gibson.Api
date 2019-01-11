using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;

namespace TechDevs.Accounts.Repositories
{
    public class CustomerRepository : AuthUserBaseRepository<Customer>
    {
        public CustomerRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser) : base(dbSettings, normaliser)
        {
        }

        public override async Task<Customer> FindById(string id, string clientId)
        {
            var result = await base.FindById(id, clientId);
            if(result?.CustomerData?.MarketingPreferences == null) result.CustomerData.MarketingPreferences = new MarketingNotificationPreferences();
            if(result?.CustomerData?.NotificationPreferences == null) result.CustomerData.NotificationPreferences = new CustomerNotificationPreferences();
            return result;
        }

        public override async Task<Customer> FindByEmail(string email, string clientId)
        {
            var result = await base.FindByEmail(email, clientId);
            if(result?.CustomerData?.MarketingPreferences == null) result.CustomerData.MarketingPreferences = new MarketingNotificationPreferences();
            if(result?.CustomerData?.NotificationPreferences == null) result.CustomerData.NotificationPreferences = new CustomerNotificationPreferences();
            return result;
        }
    }
}
