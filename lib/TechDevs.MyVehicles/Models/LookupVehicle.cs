using System.Collections.Generic;

namespace TechDevs.MyVehicles
{   
    public class LookupVehicle
    {
        public string registration { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string firstUsedDate { get; set; }
        public string fuelType { get; set; }
        public string primaryColour { get; set; }
        public string motTestDueDate {get;set;}
        public List<MotTest> motTests { get; set; }

        public LookupVehicle()
        {
            motTests = new List<MotTest>();
        }
    }
}