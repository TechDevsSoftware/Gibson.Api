using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Gibson.CustomerVehicles
{
    public class VehicleDataService : IVehicleDataService
    {
        public async Task<VehicleData> GetVehicleData(string registration)
        {
            //Sanitize the registration
            registration = registration.Replace(" ", "").ToUpper();

            HttpClient client = new HttpClient();

            var builder = new UriBuilder($"https://beta.check-mot.service.gov.uk/trade/vehicles/mot-tests");
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["registration"] = registration;
            builder.Query = query.ToString();
            var url = builder.ToString();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+v3"));

            client.DefaultRequestHeaders.Add("x-api-key", "CNGtebOnrv4DZABOoKLdQaEPWzt11GjP59LHNQEZ");

            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var strResult = await response.Content.ReadAsStringAsync();
                var lookupVehicles = JsonConvert.DeserializeObject<List<VehicleData>>(strResult);
                var lookupVehicle = lookupVehicles.FirstOrDefault();
                return lookupVehicle;
            }

            return null;
        }
    }
}
