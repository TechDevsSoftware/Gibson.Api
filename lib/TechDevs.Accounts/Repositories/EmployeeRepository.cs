using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;

namespace TechDevs.Accounts.Repositories
{
    public class EmployeeRepository : AuthUserBaseRepository<Employee>
    {
        public EmployeeRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser) : base(dbSettings, normaliser)
        {
        }
    }
}
