using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.MyVehicles
{
    public interface IMyVehicleService
    {
        Task<Customer> AddVehicle(CustomerVehicle vehicle, string userId, string clientId);
        Task<Customer> RemoveVehicle(string registration, string userId, string clientId);
        Task<Customer> UpdateVehicleMOTData(string registration, string userId, string clientId);
        Task<CustomerVehicle> LookupVehicle(string registration);
    }

}