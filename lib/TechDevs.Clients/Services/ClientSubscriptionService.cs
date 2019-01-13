using System;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Clients
{
    public interface IClientSubscriptionService
    {
        Task<Client> SetCurrentPackage(Product product, string clientId);
        Task<Client> SetSubscriptionStatus(SubscriptionStatus status, string clientId);
    }

    public class ClientSubscriptionService : IClientSubscriptionService
    {
        private readonly IClientRepository _clientRepo;

        public ClientSubscriptionService(IClientRepository clientRepo)
        {
            this._clientRepo = clientRepo;
        }

        public async Task<Client> SetCurrentPackage(Product product, string clientId)
        {
            var client = await _clientRepo.GetClientByShortKey(clientId);
            if(client == null) throw new Exception("Client not found");

            if(client.Subscription == null) client.Subscription = new ClientSubscription();
            client.Subscription.ActivePackage = product;

            var result = await _clientRepo.UpdateClient("Subscription", client.Subscription, clientId);
            return result;
        }

        public async Task<Client> SetSubscriptionStatus(SubscriptionStatus status, string clientId)
        {
            var client = await _clientRepo.GetClientByShortKey(clientId);
            if(client == null) throw new Exception("Client not found");

            if(client.Subscription == null) client.Subscription = new ClientSubscription();
            client.Subscription.SubscriptionStatus = status;
            
            var result = await _clientRepo.UpdateClient("Subscription", client.Subscription, clientId);
            return result;
        }
    }
}