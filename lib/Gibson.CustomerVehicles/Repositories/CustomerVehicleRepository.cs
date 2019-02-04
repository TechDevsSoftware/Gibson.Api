using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;

namespace Gibson.CustomerVehicles
{

    public class CustomerVehicleRespository : CustomerDataRepository<CustomerVehicle>, ICustomerVehicleRepository
    {
        public CustomerVehicleRespository(IOptions<MongoDbSettings> dbSettings)
        : base("CustomerVehicles", dbSettings)
        {
        }
    }
}
