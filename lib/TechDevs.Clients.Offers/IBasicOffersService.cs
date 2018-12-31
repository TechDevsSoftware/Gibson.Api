using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Clients.Offers
{
    public interface IBasicOffersService
    {
        Task<Client> UpdateBasicOffer(BasicOffer offer, string clientId);
        Task<Client> DeleteBasicOffer(string offerId, string clientId);
    }
}