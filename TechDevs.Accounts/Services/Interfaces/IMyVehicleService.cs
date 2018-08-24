using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public interface IMyVehicleService
    {
        Task<Customer> AddVehicle(CustomerVehicle vehicle, string userId);
        Task<Customer> RemoveVehicle(string registration, string userId);
        Task<CustomerVehicle> LookupVehicle(string registration);
    }

}