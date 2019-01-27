using Microsoft.Extensions.Options;
using TechDevs.Mail;
using TechDevs.Shared.Models;
using TechDevs.Users;
using TechDevs.Clients;

namespace TechDevs.Employees
{
    public class EmployeeService : UserService<Employee>, IEmployeeService
    {
        public EmployeeService(
            IAuthUserRepository<Employee> userRepo, 
            IPasswordHasher passwordHasher, 
            IEmailer emailer, 
            IOptions<AppSettings> appSettings,
            IClientService clientService,
            IAuthTokenService<Employee> tokenService)
            : base(userRepo, passwordHasher, emailer, appSettings, clientService, tokenService)
        {

        }
    }

}