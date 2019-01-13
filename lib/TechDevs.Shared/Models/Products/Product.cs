using System;
using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float MonthlyPrice { get; set; }
        public float AnualPrice { get; set; }
        public bool IsPackage { get; set; }
        public List<Product> PackageProducts { get; set; }

        public Product()
        {
            Id = Guid.NewGuid().ToString();
            PackageProducts = new List<Product>();
        }
    }
}
