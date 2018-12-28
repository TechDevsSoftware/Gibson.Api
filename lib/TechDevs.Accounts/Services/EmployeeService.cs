using Microsoft.Extensions.Options;
using TechDevs.Accounts.Repositories;
using TechDevs.Mail;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts
{
    public class EmployeeService : AuthUserService<Employee>
    {
        public EmployeeService(IAuthUserRepository<Employee> userRepo, IPasswordHasher passwordHasher, IEmailer emailer, IOptions<AppSettings> appSettings)
            : base(userRepo, passwordHasher, emailer, appSettings)
        {
        }
    }

}