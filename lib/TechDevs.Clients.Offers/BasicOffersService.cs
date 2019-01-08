using System;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Clients.Offers
{
    public class BasicOffersService : IBasicOffersService
    {
        private readonly IClientRepository _clientRepository;

        public BasicOffersService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client> UpdateBasicOffer(BasicOffer offer, string clientId)
        {
            // Get the client and the existing offer
            var client = await _clientRepository.GetClient(clientId);

            if(client.ClientData == null) {
                client.ClientData = new ClientData();
                client.ClientData.BasicOffers = new System.Collections.Generic.List<BasicOffer>();
            }

            var existingOfferIndex = client?.ClientData?.BasicOffers.FindIndex(x => x.Id == offer.Id);
            if (!existingOfferIndex.HasValue || existingOfferIndex.Value == -1)
            {
                // No existing offer found, so add the new basic offer
                offer.Id = Guid.NewGuid().ToString();
                client.ClientData.BasicOffers.Add(offer);
            }
            else
            {
                // Replace the existing offer
                client.ClientData.BasicOffers[existingOfferIndex.Value] = offer;
            }
            var result = await _clientRepository.UpdateClient("ClientData.BasicOffers", client.ClientData.BasicOffers, client.Id);
            return result;
        }

        public async Task<Client> DeleteBasicOffer(string offerId, string clientId)
        {
            // Get the client and the existing offer
            var client = await _clientRepository.GetClient(clientId);
            var existingOfferIndex = client?.ClientData?.BasicOffers.FindIndex(x => x.Id == offerId);
            if (!existingOfferIndex.HasValue || existingOfferIndex.Value == -1) throw new Exception("Offer could not be found");
            // Replace the existing offer
            client.ClientData.BasicOffers.RemoveAt(existingOfferIndex.Value);
            // Update the clinet and return
            var result = await _clientRepository.UpdateClient("ClientData.BasicOffers", client.ClientData.BasicOffers, client.Id);
            return result;
        }
    }
}
