using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Clients.Offers
{
    public interface IOffersService
    {
        Task<List<BasicOffer>> GetActiveOffers(Guid clientId);
        Task<List<BasicOffer>> GetOffers(Guid clientId);
        Task<BasicOffer> CreateOffer(BasicOffer offer, Guid clientId);
        Task<BasicOffer> UpdateOffer(BasicOffer offer, Guid clientId);
        Task DeleteOffer(Guid offerId, Guid clientId);
    }
}