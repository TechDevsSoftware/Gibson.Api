using System;

namespace TechDevs.Shared.Models
{
    public class BookingCustomer
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }

    }
}
