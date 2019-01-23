using Microsoft.Extensions.Options;
using TechDevs.Mail;
using TechDevs.Shared.Models;
using TechDevs.Users;
using TechDevs.Clients;

namespace TechDevs.Employees
{
    public class EmployeeService : AuthUserService<Employee>
    {
        public EmployeeService(
            IAuthUserRepository<Employee> userRepo, 
            IPasswordHasher passwordHasher, 
            IEmailer emailer, 
            IOptions<AppSettings> appSettings,
            IClientService clientService)
            : base(userRepo, passwordHasher, emailer, appSettings, clientService)
        {

        }
    }

}