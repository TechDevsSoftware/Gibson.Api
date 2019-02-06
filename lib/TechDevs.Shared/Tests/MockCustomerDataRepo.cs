using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;

namespace Gibson.CustomerVehicles
{
    public class MockCustomerDataRepo : CustomerDataRepository<MockCustomerEntity>
    {
        public MockCustomerDataRepo(IOptions<MongoDbSettings> dbSettings)
        : base("DummyCollection", dbSettings) { }
    }
}
