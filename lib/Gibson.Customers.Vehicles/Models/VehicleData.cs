using System;
using System.Collections.Generic;

namespace Gibson.Customers.Vehicles
{
    public class VehicleData
    {
        public string registration { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string firstUsedDate { get; set; }
        public string fuelType { get; set; }
        public string primaryColour { get; set; }
        public string motTestDueDate { get; set; }
        public List<LookupMotTest> motTests { get; set; }

        public VehicleData()
        {
            motTests = new List<LookupMotTest>();
        }
    }

    public class LookupMotTest
    {
        public string completedDate { get; set; }
        public string testResult { get; set; }
        public string expiryDate { get; set; }
        public string odometerValue { get; set; }
        public string odometerUnit { get; set; }
        public string odometerResultType { get; set; }
        public string motTestNumber { get; set; }
        public List<LookupMotComment> rfrAndComments { get; set; }

        public LookupMotTest()
        {
            rfrAndComments = new List<LookupMotComment>();
        }
    }

    public class LookupMotComment
    {
        public string text { get; set; }
        public string type { get; set; }
    }
}
