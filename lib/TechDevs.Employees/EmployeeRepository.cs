using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;
using TechDevs.Users;

namespace TechDevs.Employees
{
    public class EmployeeRepository : AuthUserBaseRepository<Employee>
    {
        public EmployeeRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser) : base(dbSettings, normaliser)
        {
        }
    }
}
