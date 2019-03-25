using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Gibson.Common.Models;

namespace Gibson.Clients.Offers
{
    public class OffersService : IOffersService
    {
        private readonly IRepository<BasicOffer> _offerRepository;

        public OffersService(IRepository<BasicOffer> offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<List<BasicOffer>> GetActiveOffers(Guid clientId)
        {
            var allOffers = await GetOffers(clientId);
            var result = allOffers.Where(x => x.Enabled).ToList();
            return result;
        }

        public async Task<List<BasicOffer>> GetOffers(Guid clientId)
        {
            var allOffers = await _offerRepository.FindAll(clientId);
            return allOffers;
        }

        public async Task<BasicOffer> CreateOffer(BasicOffer offer, Guid clientId)
        {
            var result = await _offerRepository.Create(offer, clientId);
            return result;
        }
    
        public async Task<BasicOffer> UpdateOffer(BasicOffer offer, Guid clientId)
        {
            var result = await _offerRepository.Update(offer, clientId);
            return result;
        }

        public async Task DeleteOffer(Guid offerId, Guid clientId)
        {
            await _offerRepository.Delete(offerId, clientId);
        }
    }
}
