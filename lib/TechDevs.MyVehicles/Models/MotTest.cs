using System.Collections.Generic;

namespace TechDevs.MyVehicles
   {
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

        public MotTest()
        {
            rfrAndComments = new List<RawMotComment>();
        }
    }
   }