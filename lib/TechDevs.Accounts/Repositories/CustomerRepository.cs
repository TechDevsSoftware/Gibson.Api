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
    }
}
