using System.Threading.Tasks;
using TechDevs.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Gibson.CustomerVehicles
{
    public class CustomerVehicleService : ICustomerVehicleService
    {
        private readonly ICustomerVehicleRepository repo;
        private readonly IVehicleDataService vehicleData;

        public CustomerVehicleService(ICustomerVehicleRepository repo, IVehicleDataService vehicleData)
        {
            this.repo = repo;
            this.vehicleData = vehicleData;
        }

        public async Task<CustomerVehicle> AddVehicleToCustomer(string registration, Guid customerId, Guid clientId)
        {
            var vehicles = await repo.FindAllByCustomer(customerId, clientId);
            var existing = vehicles.FirstOrDefault(x => x.Registration == registration);
            if (existing != null) throw new Exception("Vehicle with this registration already exists");

            var lookup = await vehicleData.GetVehicleData(registration);
            var vehicle = MapLookupToCustomerVehicle(lookup);
            var result = await repo.Create(vehicle, customerId, clientId);
            return result;
        }

        public async Task DeleteCustomerVehicle(Guid vehicleId, Guid customerId, Guid clientId)
        {
            var vehicle = await repo.FindById(vehicleId, clientId);
            if (vehicle == null) throw new Exception("Cannot delete as vehicle cannot be found");
            await repo.Delete(vehicleId, clientId);
        }

        public async Task<CustomerVehicle> GetCustomerVehicle(string registration, Guid customerId, Guid clientId)
        {
            var vehicles = await repo.FindAllByCustomer(customerId, clientId);
            var result = vehicles.FirstOrDefault(x => x.Registration == registration);
            return result;
        }

        public async Task<CustomerVehicle> GetCustomerVehicle(Guid vehicleId, Guid clientId)
        {
            var result = await repo.FindById(vehicleId, clientId);
            return result;
        }

        public async Task<List<CustomerVehicle>> GetCustomerVehicles(Guid customerId, Guid clientId)
        {
            var vehicles = await repo.FindAllByCustomer(customerId, clientId);
            return vehicles;
        }

        public async Task<CustomerVehicle> UpdateCustomerVehicle(CustomerVehicle vehicle, Guid clientId)
        {
            var existing = await repo.FindById(vehicle.Id, clientId);
            if (existing == null) throw new Exception("Vehicle cannot be found");
            var result = await repo.Update(vehicle, clientId);
            return result;
        }

        public async Task<CustomerVehicle> UpdateMotData(Guid vehicleId, Guid clientId)
        {
            var vehicle = await repo.FindById(vehicleId, clientId);
            var lookup = await vehicleData.GetVehicleData(vehicle.Registration);
            var motData = MapLookupToCustomerVehicle(lookup)?.MotData;
            vehicle.MotData = motData;
            var result = await repo.Update(vehicle, clientId);
            return result;
        }

        public async Task<CustomerVehicle> UpdateServiceData(ServiceData serviceData, Guid vehicleId, Guid clientId)
        {
            var vehicle = await repo.FindById(vehicleId, clientId);
            if (vehicle == null) throw new Exception("Vehicle could not be found");
            vehicle.ServiceData = serviceData;
            var result = await UpdateCustomerVehicle(vehicle, clientId);
            return result;
        }

        private DateTime? CalculateMOTExpiry(VehicleData v)
        {
            string strDate = v?.motTestDueDate ?? v?.motTests.FirstOrDefault()?.expiryDate;
            if (string.IsNullOrEmpty(strDate)) return null;
            DateTime.TryParse(strDate, out DateTime result);
            return result;
        }

        private DateTime? CalculateServiceExpiry(CustomerVehicle v)
        {
            // Max Mileage
            // Max Months
            // Est Anual Mileage
            var maxMiles = v.ServiceData?.MaxMileage;
            var maxMonths = v.ServiceData?.MaxMonths;
            var estMiles = v.ServiceData?.MaxMileage;

            // When was the last service done
            // Show the user remianing miles / days / months
            // Offer them the ability to re-calculate their service renewal

            return null;
             
        }

        private CustomerVehicle MapLookupToCustomerVehicle(VehicleData lookup)
        {
            return new CustomerVehicle
            {
                Registration = lookup?.registration,
                Make = lookup?.make,
                Model = lookup?.model,
                FuelType = lookup?.fuelType,
                Colour = lookup?.primaryColour,
                Year = (lookup?.firstUsedDate != null) ? int.Parse(lookup.firstUsedDate.Substring(0, 4)) : 0,
                LastUpdated = DateTime.UtcNow,
                MotData = new MotData
                {
                    MOTExpiryDate = CalculateMOTExpiry(lookup),
                    MOTResults = (lookup.motTests == null || lookup.motTests.Count == 0) ? new List<MotResult>() : lookup.motTests.Select(mot => new MotResult
                    {
                        TestResult = mot.testResult,
                        CompletedDate = mot.completedDate,
                        ExpiryDate = mot.expiryDate,
                        MotTestNumber = mot.motTestNumber,
                        OdometerResultType = mot.odometerResultType,
                        OdometerUnit = mot.odometerUnit,
                        OdometerValue = mot.odometerValue,
                        Comments = (mot?.rfrAndComments == null) ? new List<MotComment>() : mot.rfrAndComments.Select(c => new MotComment
                        {
                            Text = c.text,
                            Type = c.type
                        }).ToList()
                    }).ToList()
                },
                ServiceData = new ServiceData { }
            };
        }
    }
}