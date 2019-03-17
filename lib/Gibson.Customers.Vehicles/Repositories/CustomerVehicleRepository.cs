using Gibson.Shared.Repositories;
using Microsoft.Extensions.Options;
using Gibson.Common.Models;

namespace Gibson.Customers.Vehicles
{

    public class CustomerVehicleRespository : CustomerDataRepository<CustomerVehicle>, ICustomerVehicleRepository
    {
        public CustomerVehicleRespository(IOptions<MongoDbSettings> dbSettings)
        : base("CustomerVehicles", dbSettings)
        {
        }
    }
}
