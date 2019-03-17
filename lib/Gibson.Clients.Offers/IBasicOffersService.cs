using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Clients.Offers
{
    public interface IBasicOffersService
    {
        Task<Client> UpdateBasicOffer(BasicOffer offer, string clientId);
        Task<Client> DeleteBasicOffer(string offerId, string clientId);
    }
}