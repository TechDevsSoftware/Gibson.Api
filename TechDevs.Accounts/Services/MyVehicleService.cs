using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace TechDevs.Accounts
{
    public class MyVehicleService : IMyVehicleService
    {
        private readonly IUserRepository _userRepo;

        public MyVehicleService(IUserRepository userRepository)
        {
            _userRepo = userRepository;
        }

        public async Task<IUser> AddVehicle(UserVehicle vehicle, string userId)
        {
            var user = await _userRepo.FindById(userId);
            if (user == null) throw new Exception("User not found");

            if (user.UserData.MyVehicles.Any(v => v.Registration == vehicle.Registration)) throw new Exception("Vehicle already added. Cannot duplicate vehicle registrations.");

            user.UserData.MyVehicles.Add(vehicle);

            var result = await _userRepo.UpdateUser<UserVehicle>("UserData.MyVehicles", user.UserData.MyVehicles, userId);
            return result;
        }

        public async Task<IUser> RemoveVehicle(string registration, string userId)
        {
            var user = await _userRepo.FindById(userId);
            if (user == null) throw new Exception("User not found");

            user.UserData.MyVehicles = user.UserData.MyVehicles.Where(x => x.Registration != registration).ToList();

            var result = await _userRepo.UpdateUser<UserVehicle>("UserData.MyVehicles", user.UserData.MyVehicles, userId);
            return result;
        }

        public async Task<UserVehicle> LookupVehicle(string registration)
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
                var vehicle = new UserVehicle
                {
                    Registration = lookupVehicle.registration,
                    Make = lookupVehicle.make,
                    Model = lookupVehicle.model,
                    FuelType = lookupVehicle.fuelType,
                    Colour = lookupVehicle.primaryColour,
                    MOTResults = (lookupVehicle.motTests == null) ? new List<MotResult>() : lookupVehicle.motTests.Select(x => new MotResult
                    {
                        TestResult = x.testResult,
                        CompletedDate = x.completedDate,
                        ExpiryDate = x.expiryDate,
                        MotTestNumber = x.motTestNumber,
                        OdometerResultType = x.odometerResultType,
                        OdometerUnit = x.odometerUnit,
                        OdometerValue = x.odometerValue
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
        public List<object> rfrAndComments { get; set; }
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