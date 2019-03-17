using System.Collections.Generic;

namespace Gibson.Common.Models
{
    public class MotResult
    {
        public string CompletedDate { get; set; }
        public string TestResult { get; set; }
        public string ExpiryDate { get; set; }
        public string OdometerValue { get; set; }
        public string OdometerUnit { get; set; }
        public string OdometerResultType { get; set; }
        public string MotTestNumber { get; set; }
        public List<MotComment> Comments { get; set; }
    }
}