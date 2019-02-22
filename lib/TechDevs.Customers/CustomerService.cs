using Gibson.AuthTokens;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using TechDevs.Clients;
using TechDevs.Shared.Models;
using TechDevs.Users;

namespace TechDevs.Customers
{
    public class CustomerService : UserService<Customer>, ICustomerService
    {
        public CustomerService(
            IAuthUserRepository<Customer> userRepo,
            IPasswordHasher passwordHasher,
            IOptions<AppSettings> appSettings,
            IClientService clientService,
            IAuthTokenService tokenService)
            : base(userRepo, passwordHasher, appSettings, clientService, tokenService)
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