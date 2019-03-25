using System;
using System.Linq;
using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Clients.Offers
{
    public class OffersService : IOffersService
    {
        private readonly IClientRepository _clientRepository;

        public OffersService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // If ClientData IsNull New Up A new client Data

        public async Task<BasicOffer> UpdateBasicOffer(BasicOffer offer, string clientId)
        {
            // Get the client and the existing offer
            var client = await _clientRepository.GetClient(clientId);

            if (client.ClientData == null)
            {
                client.ClientData = new ClientData
                {
                    BasicOffers = new System.Collections.Generic.List<BasicOffer>()
                };
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
            var offerResult = result.ClientData.BasicOffers.FirstOrDefault(x => x.Id == offer.Id);
            return offerResult;
        }

        // If Offer cannot be found throw exception

        public async Task DeleteBasicOffer(string offerId, string clientId)
        {
            // Get the client and the existing offer
            var client = await _clientRepository.GetClient(clientId);
            var existingOfferIndex = client?.ClientData?.BasicOffers.FindIndex(x => x.Id == offerId);
            if (!existingOfferIndex.HasValue || existingOfferIndex.Value == -1) throw new Exception("Offer could not be found");
            // Replace the existing offer
            client.ClientData.BasicOffers.RemoveAt(existingOfferIndex.Value);
            // Update the client and return
            await _clientRepository.UpdateClient("ClientData.BasicOffers", client.ClientData.BasicOffers, client.Id);

        }
    }
}
