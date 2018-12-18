using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using TechDevs.Accounts.Repositories;

namespace TechDevs.Accounts
{
    public class MyVehicleService : IMyVehicleService
    {
        private readonly IAuthUserRepository<Customer> _userRepo;

        public MyVehicleService(IAuthUserRepository<Customer> userRepository)
        {
           _userRepo = userRepository;
        }

        public async Task<Customer> AddVehicle(CustomerVehicle vehicle, string userId, string clientId)
        {
           var user = await _userRepo.FindById(userId, clientId);
           if (user == null) throw new Exception("User not found");

           if (user.CustomerData.MyVehicles.Any(v => v.Registration == vehicle.Registration)) throw new Exception("Vehicle already added. Cannot duplicate vehicle registrations.");

           user.CustomerData.MyVehicles.Add(vehicle);

           var result = await _userRepo.UpdateUser<CustomerVehicle>("CustomerData.MyVehicles", user.CustomerData.MyVehicles, userId, clientId);
           return result;
        }

        public async Task<Customer> RemoveVehicle(string registration, string userId, string clientId)
        {
           var user = await _userRepo.FindById(userId, clientId);
           if (user == null) throw new Exception("User not found");

           user.CustomerData.MyVehicles = user.CustomerData.MyVehicles.Where(x => x.Registration != registration).ToList();

           var result = await _userRepo.UpdateUser<CustomerVehicle>("CustomerData.MyVehicles", user.CustomerData.MyVehicles, userId, clientId);
           return result;
        }

        public async Task<CustomerVehicle> LookupVehicle(string registration)
        {
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
               var lookupVehicles = JsonConvert.DeserializeObject<List<LookupVehicle>>(strResult);
               var lookupVehicle = lookupVehicles.FirstOrDefault();
               var vehicle = new CustomerVehicle
               {
                   Registration = lookupVehicle.registration,
                   Make = lookupVehicle.make,
                   Model = lookupVehicle.model,
                   FuelType = lookupVehicle.fuelType,
                   Colour = lookupVehicle.primaryColour,
                   Year = int.Parse(lookupVehicle.firstUsedDate.Substring(0,4)),
                   MOTResults = (lookupVehicle.motTests == null) ? new List<MotResult>() : lookupVehicle.motTests.Select(x => new MotResult
                   {
                       TestResult = x.testResult,
                       CompletedDate = x.completedDate,
                       ExpiryDate = x.expiryDate,
                       MotTestNumber = x.motTestNumber,
                       OdometerResultType = x.odometerResultType,
                       OdometerUnit = x.odometerUnit,
                       OdometerValue = x.odometerValue,
                       Comments = (x.rfrAndComments == null) ? new List<MotComment>() : x.rfrAndComments.Select(c => new MotComment
                       {
                           Text = c.text,
                           Type = c.type
                       }).ToList()                   
                   }).ToList()
               };


               return vehicle;
           }
            
           return null;
        }
    }

    public class MotTest
    {
        public string completedDate { get; set; }
        public string testResult { get; set; }
        public string expiryDate { get; set; }
        public string odometerValue { get; set; }
        public string odometerUnit { get; set; }
        public string odometerResultType { get; set; }
        public string motTestNumber { get; set; }
        public List<RawMotComment> rfrAndComments { get; set; }
    }

    public class RawMotComment
    {
        public string text {get;set;}
        public string type {get;set;}
    }

    public class LookupVehicle
    {
        public string registration { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string firstUsedDate { get; set; }
        public string fuelType { get; set; }
        public string primaryColour { get; set; }
        public List<MotTest> motTests { get; set; }
    }
}