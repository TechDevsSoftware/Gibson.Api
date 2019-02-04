//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using System.Web;
//using Newtonsoft.Json;
//using TechDevs.Shared.Models;
//using TechDevs.Users;

//namespace TechDevs.MyVehicles
//{
//    public class MyVehicleService : IMyVehicleService
//    {
//        private readonly IUserService<Customer> users;
//        private readonly IAuthUserRepository<Customer> customerRepo;

//        public MyVehicleService(IUserService<Customer> users, IAuthUserRepository<Customer> customerRepo)
//        {
//            this.users = users;
//            this.customerRepo = customerRepo;
//        }

//        public async Task<CustomerVehicle> GetCustomerVehicle(string registration, string userId, string clientKeyOrId)
//        {
//            var user = await users.GetById(userId, clientKeyOrId);
//            if (user == null) throw new Exception("User not found");

//            // Sanitize the registration
//            registration = registration.Replace(" ", "").ToUpper();

//            var existingVehicle = user.CustomerData.MyVehicles.Find(x => x.Registration == registration);
//            return existingVehicle;
//        }

//        public async Task<Customer> AddVehicle(CustomerVehicle vehicle, string userId, string clientId)
//        {
//            var user = await users.GetById(userId, clientId);
//            if (user == null) throw new Exception("User not found");

//            // Sanitize the registration
//            vehicle.Registration = vehicle.Registration.Replace(" ", "").ToUpper();

//            if (user.CustomerData.MyVehicles.Any(v => v.Registration == vehicle.Registration)) throw new Exception("Vehicle already added. Cannot duplicate vehicle registrations.");
            
//            user.CustomerData.MyVehicles.Add(vehicle);

//            var result = await customerRepo.UpdateUser("CustomerData.MyVehicles", user.CustomerData.MyVehicles, userId, clientId);
//            return result;
//        }

//        public async Task<Customer> RemoveVehicle(string registration, string userId, string clientId)
//        {
//            var user = await users.GetById(userId, clientId);
//            if (user == null) throw new Exception("User not found");

//            // Sanitize the registration
//            registration = registration.Replace(" ", "").ToUpper();

//            user.CustomerData.MyVehicles = user.CustomerData.MyVehicles.Where(x => x.Registration != registration).ToList();

//            var result = await customerRepo.UpdateUser("CustomerData.MyVehicles", user.CustomerData.MyVehicles, userId, clientId);
//            return result;
//        }

//        public async Task<Customer> UpdateVehicleMOTData(string registration, string userId, string clientId)
//        {
//            var user = await users.GetById(userId, clientId);
//            if (user == null) throw new Exception("User not found");

//            // Sanitize the registration
//            registration = registration.Replace(" ", "").ToUpper();

//            var existingVehicle = user.CustomerData.MyVehicles.Find(x => x.Registration == registration);
//            var latestMOTData = await LookupVehicle(registration);
//            if(existingVehicle != latestMOTData)
//            {
//                user.CustomerData.MyVehicles[user.CustomerData.MyVehicles.FindIndex(x => x.Registration == registration)] = latestMOTData;
//                var result = await customerRepo.UpdateUser("CustomerData.MyVehicles", user.CustomerData.MyVehicles, userId, clientId);
//                return result;
//            }
//            return user;
//        }

//        public async Task<CustomerVehicle> LookupVehicle(string registration)
//        {
//            // Sanitize the registration
//            registration = registration.Replace(" ", "").ToUpper();

//            HttpClient client = new HttpClient();

//            var builder = new UriBuilder($"https://beta.check-mot.service.gov.uk/trade/vehicles/mot-tests");
//            var query = HttpUtility.ParseQueryString(string.Empty);
//            query["registration"] = registration;
//            builder.Query = query.ToString();
//            var url = builder.ToString();
//            client.DefaultRequestHeaders.Accept.Clear();
//            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json+v3"));

//            client.DefaultRequestHeaders.Add("x-api-key", "CNGtebOnrv4DZABOoKLdQaEPWzt11GjP59LHNQEZ");

//            HttpResponseMessage response = await client.GetAsync(url);
//            if (response.IsSuccessStatusCode)
//            {
//                var strResult = await response.Content.ReadAsStringAsync();
//                var lookupVehicles = JsonConvert.DeserializeObject<List<LookupVehicle>>(strResult);
//                var lookupVehicle = lookupVehicles.FirstOrDefault();
//                var vehicle = new CustomerVehicle
//                {
//                    Registration = lookupVehicle?.registration,
//                    Make = lookupVehicle?.make,
//                    Model = lookupVehicle?.model,
//                    FuelType = lookupVehicle?.fuelType,
//                    Colour = lookupVehicle?.primaryColour,
//                    Year = (lookupVehicle?.firstUsedDate != null) ? int.Parse(lookupVehicle.firstUsedDate.Substring(0, 4)) : 0,
//                    MOTExpiryDate = CalculateMOTExpiry(lookupVehicle),
//                    MOTResults = (lookupVehicle.motTests == null || lookupVehicle.motTests.Count == 0) ? new List<MotResult>() : lookupVehicle.motTests.Select(mot => new MotResult
//                    {
//                        TestResult = mot.testResult,
//                        CompletedDate = mot.completedDate,
//                        ExpiryDate = mot.expiryDate,
//                        MotTestNumber = mot.motTestNumber,
//                        OdometerResultType = mot.odometerResultType,
//                        OdometerUnit = mot.odometerUnit,
//                        OdometerValue = mot.odometerValue,
//                        Comments = (mot?.rfrAndComments == null) ? new List<MotComment>() : mot.rfrAndComments.Select(c => new MotComment
//                        {
//                            Text = c.text,
//                            Type = c.type
//                        }).ToList()
//                    }).ToList()
//                };
//                vehicle.LastUpdated = DateTime.UtcNow;
//                return vehicle;
//            }

//            return null;
//        }

//        private DateTime? CalculateMOTExpiry(LookupVehicle v)
//        {
//            string strDate = v?.motTestDueDate ?? v?.motTests.FirstOrDefault()?.expiryDate;
//            if(string.IsNullOrEmpty(strDate)) return null;
//            DateTime result;
//            DateTime.TryParse(strDate, out result);
//            return result;
//        }
//    }
//}