using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Customers.Vehicles
{
    public interface IVehicleDataService
    {
        Task<VehicleData> GetVehicleData(string registration);
    }
}