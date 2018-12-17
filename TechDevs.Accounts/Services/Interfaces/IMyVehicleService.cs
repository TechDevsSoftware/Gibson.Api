using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public interface IMyVehicleService
    {
        Task<Customer> AddVehicle(CustomerVehicle vehicle, string userId, string clientId);
        Task<Customer> RemoveVehicle(string registration, string userId, string clientId);
        Task<CustomerVehicle> LookupVehicle(string registration);
    }

}