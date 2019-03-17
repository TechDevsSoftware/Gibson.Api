using System;

namespace Gibson.Common.Models
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