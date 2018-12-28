using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TechDevs.Accounts.Repositories;
using TechDevs.Mail;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts
{
    public class CustomerService : AuthUserService<Customer>, ICustomerService
    {
        public CustomerService(IAuthUserRepository<Customer> userRepo, IPasswordHasher passwordHasher, IEmailer emailer, IOptions<AppSettings> appSettings)
            : base(userRepo, passwordHasher, emailer, appSettings)
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