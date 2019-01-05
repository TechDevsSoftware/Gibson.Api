using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts
{
    public interface ICustomerService : IAuthUserService<Customer>
    {
        Task<Customer> UpdateCustomerData<CustDataType>(string userId, string customerDataPathName, CustDataType custData, string clientId);
    }
}