using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Clients.Offers
{
    public interface IOffersService
    {
        Task<BasicOffer> UpdateBasicOffer(BasicOffer offer, string clientId);
        Task DeleteBasicOffer(string offerId, string clientId);
    }
}