using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using TechDevs.Users;
using TechDevs.Clients;
using Gibson.AuthTokens;

namespace TechDevs.Employees
{
    public class EmployeeService : UserService<Employee>, IEmployeeService
    {
        public EmployeeService(
            IAuthUserRepository<Employee> userRepo, 
            IPasswordHasher passwordHasher, 
            IOptions<AppSettings> appSettings,
            IClientService clientService,
            IAuthTokenService tokenService)
            : base(userRepo, passwordHasher, appSettings, clientService, tokenService)
        {
        }
    }

}