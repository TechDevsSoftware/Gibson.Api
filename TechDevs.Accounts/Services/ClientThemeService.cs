using System;
using System.Threading.Tasks;
using TechDevs.Accounts.Repositories;

namespace TechDevs.Accounts.Services
{
    public class ClientThemeService : IClientThemeService
    {
        private readonly IClientRepository _clientRepo;

        public ClientThemeService(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public async Task<Client> SetParameter(string clientId, string key, string value)
        {
            // Delete any existing confilcting params
            await RemoveParameter(clientId, key);
            
            var client = await _clientRepo.GetClient(clientId);
            if (client == null) throw new Exception("Client could not be found");

            // Add the new parameter
            client.ClientTheme.Parameters.Add(new CSSParameter { Key = key, Value = value });

            // Update the DB
            var result = await _clientRepo.UpdateClient(clientId, client);

            return result;
        }
        public async Task<Client> RemoveParameter(string clientId, string key)
        {
            var client = await _clientRepo.GetClient(clientId);
            if (client == null) throw new Exception("Client could not be found");

            if (client.ClientTheme == null) 
            {
                client.ClientTheme = new ClientTheme();
            }

            if (client.ClientTheme.Parameters == null)
            {
                client.ClientTheme.Parameters = new System.Collections.Generic.List<CSSParameter>();
            }

            // Remove any parameters already there
            client.ClientTheme.Parameters.RemoveAll(x => x.Key == key);

            var result = await _clientRepo.UpdateClient(clientId, client);
            return result;
        }
    }
}
