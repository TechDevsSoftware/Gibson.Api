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
        public MotData MotData { get; set; }
        public ServiceData ServiceData { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class MotData
    {
        public DateTime? MOTExpiryDate { get; set; }
        public List<MotResult> MOTResults { get; set; }
    }

    public class ServiceData
    {
        public DateTime? LastServicedOn { get; set; }
        public DateTime? CalculatedServiceDue { get; set; }
        public int MaxMonths { get; set; }
        public int MaxMileage { get; set; }
        public int EstAnualMileage { get; set; }
        public int CalculatedAnualMileage { get; set; }
        public string ServiceDataConfiguredBy { get; set; }
    }

    public class ServiceHistroy
    {

    }
}