using Gibson.Common.Models;
using Gibson.Shared.Repositories;
using Microsoft.Extensions.Options;

namespace Gibson.Clients.Offers
{
    public class OffersRepository : ClientDataRepository<BasicOffer>
    {
        public OffersRepository(IOptions<MongoDbSettings> dbSettings) : base("Offers", dbSettings)
        {
        }
    }
}