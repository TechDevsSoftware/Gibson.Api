using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Gibson.Common.Models;
using Gibson.Customers.Vehicles;

namespace Gibson.Customers.Vehicles.Tests
{
    public class MockVehicleDataService : IVehicleDataService
    {
        private readonly List<VehicleData> vehicles;

        public MockVehicleDataService()
        {
            vehicles = new List<VehicleData>
            {
                new VehicleData
                {
                    registration = "EF02VCC",
                    make = "RENAULT",
                    model = "CLIO",
                    primaryColour = "WHITE",
                    fuelType = "PETROL",
                    motTests = new List<LookupMotTest>
                    {
                        new LookupMotTest
                        {
                            completedDate = "2016.10.28 09:59:42",
                            motTestNumber = "1234",
                            testResult = "PASSED",
                            expiryDate = "2020.11.06",
                            odometerUnit = "mi",
                            odometerValue = "123456",
                            rfrAndComments = new List<LookupMotComment>
                            {
                                new LookupMotComment { text = "Comment", type = "ADVISORY" }
                            }
                        }
                    }
                },
                new VehicleData
                {
                    registration = "LD66OFZ",
                    make = "KIA",
                    model = "CEED",
                    primaryColour = "BLACK",
                    fuelType = "DIESEL",
                    motTestDueDate = "2020-11-13",
                    motTests = new List<LookupMotTest>
                    {
                    }
                }
            };
        }

        public Task<VehicleData> GetVehicleData(string registration)
        {
            return Task.FromResult(vehicles.FirstOrDefault(x => x.registration == registration));
        }
    }
}