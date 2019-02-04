using System;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.CustomerVehicles
{
    public interface ICustomerVehicleService
    {
        Task<CustomerVehicle> GetCustomerVehicle(string registration, Guid customerId, Guid clientId);
        Task<CustomerVehicle> AddVehicleToCustomer(string registration, Guid customerId, Guid clientId);
        Task<CustomerVehicle> UpdateCustomerVehicle(CustomerVehicle vehicle, Guid customerId, Guid clientId);
        Task<CustomerVehicle> UpdateMotData(Guid vehicleId, Guid customerId, Guid clientId);
        Task DeleteCustomerVehicle(Guid vehicleId, Guid customerId, Guid clientId);
    }
}