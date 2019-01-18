using Microsoft.Extensions.Options;
using TechDevs.Mail;
using TechDevs.Shared.Models;
using TechDevs.Users;

namespace TechDevs.Employees
{
    public class EmployeeService : AuthUserService<Employee>
    {
        public EmployeeService(IAuthUserRepository<Employee> userRepo, IPasswordHasher passwordHasher, IEmailer emailer, IOptions<AppSettings> appSettings)
            : base(userRepo, passwordHasher, emailer, appSettings)
        {
        }
    }

}