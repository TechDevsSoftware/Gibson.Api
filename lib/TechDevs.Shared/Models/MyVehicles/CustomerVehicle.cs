using System;
using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class CustomerVehicle : CustomerEntity
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string Registration { get; set; }
        public string Colour { get; set; }
        public string FuelType { get; set; }
        public int Year { get; set; }
        public DateTime? MOTExpiryDate { get; set; }
        public List<MotResult> MOTResults { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}