using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public interface IMyVehicleService
    {
        Task<ICustomer> AddVehicle(CustomerVehicle vehicle, string userId);
        Task<ICustomer> RemoveVehicle(string registration, string userId);
        Task<CustomerVehicle> LookupVehicle(string registration);
    }

}