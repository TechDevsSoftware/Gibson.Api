using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class CustomerData
    {
        public List<CustomerVehicle> MyVehicles { get; set; }

        public CustomerData()
        {
            MyVehicles = new List<CustomerVehicle>();
        }
    }

}