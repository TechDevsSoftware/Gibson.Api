using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.CustomerVehicles
{
    public interface ICustomerVehicleService
    {
        Task<List<CustomerVehicle>> GetCustomerVehicles(Guid customerId, Guid clientId);
        Task<CustomerVehicle> GetCustomerVehicle(string registration, Guid customerId, Guid clientId);
        Task<CustomerVehicle> GetCustomerVehicle(Guid vehicleId, Guid clientId);

        Task<CustomerVehicle> AddVehicleToCustomer(string registration, Guid customerId, Guid clientId);

        Task<CustomerVehicle> UpdateCustomerVehicle(CustomerVehicle vehicle, Guid clientId);
        Task<CustomerVehicle> UpdateMotData(Guid vehicleId, Guid clientId);
        Task<CustomerVehicle> UpdateServiceData(ServiceData serviceData, Guid vehicleId, Guid clientId);

        Task DeleteCustomerVehicle(Guid vehicleId, Guid customerId, Guid clientId);
    }
}