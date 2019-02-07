using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;

namespace Gibson.Shared.Repositories.Tests
{
    public class MockCustomerDataRepo<T> : CustomerDataRepository<T> where T : CustomerEntity
    {
        public MockCustomerDataRepo(IOptions<MongoDbSettings> dbSettings)
        : base("DummyCollection", dbSettings) { }
    }
}
