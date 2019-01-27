using System.Threading.Tasks;
using TechDevs.Shared.Models;
using TechDevs.Users;

namespace TechDevs.Customers
{
    public interface ICustomerService : IUserService<Customer>
    {
        Task<Customer> UpdateCustomerData<CustDataType>(string userId, string customerDataPathName, CustDataType custData, string clientId);
    }
}