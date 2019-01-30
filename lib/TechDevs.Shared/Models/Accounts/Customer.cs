using System;

namespace TechDevs.Shared.Models
{
    public class Customer : AuthUser
    {
        public CustomerData CustomerData { get; set; }

        public Customer()
        {
            CustomerData = new CustomerData();
        }
    }


}