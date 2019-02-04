using System.Threading.Tasks;

namespace Gibson.CustomerVehicles
{
    public interface IVehicleDataService
    {
        Task<VehicleData> GetVehicleData(string registration);
    }
}