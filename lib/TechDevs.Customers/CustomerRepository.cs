using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;
using TechDevs.Users;

namespace TechDevs.Customers
{
    public class CustomerRepository : AuthUserBaseRepository<Customer>
    {
        public CustomerRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser) : base(dbSettings, normaliser)
        {
        }
    }
}
