using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TechDevs.Clients;
using TechDevs.Mail;
using TechDevs.Shared.Models;
using TechDevs.Users;

namespace TechDevs.Customers
{
    public class CustomerService : AuthUserService<Customer>, ICustomerService
    {
        public CustomerService(
            IAuthUserRepository<Customer> userRepo, 
            IPasswordHasher passwordHasher, 
            IEmailer emailer,
            IOptions<AppSettings> appSettings,
            IClientService clientService)
            : base(userRepo, passwordHasher, emailer, appSettings, clientService)
        {
        }

        public virtual async Task<Customer> UpdateCustomerData<CustDataType>(string userId, string customerDataPathName, CustDataType custData, string clientId)
        {
            var user = await _userRepo.FindById(userId, clientId);
            if (user == null) throw new Exception("User not found");
            var result = await _userRepo.UpdateUser($"CustomerData.{customerDataPathName}", custData, user.Id, clientId);
            return result;
        }
    }

}