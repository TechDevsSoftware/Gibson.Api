using Microsoft.Extensions.Options;
using TechDevs.Clients;
using TechDevs.Shared.Models;

namespace Gibson.CustomerVehicles
{
    public interface ICustomerVehicleRepositoy : ICustomerDataRepository<CustomerVehicle> { }

    public class CustomerVehicleRespository : CustomerDataRepository<CustomerVehicle>, ICustomerVehicleRepositoy
    {
        public CustomerVehicleRespository(IOptions<MongoDbSettings> dbSettings)
        : base("CustomerVehicles", dbSettings)
        {
        }
    }
}
