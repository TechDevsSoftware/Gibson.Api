using System;

namespace TechDevs.Shared.Models
{
    public class BookingRequest_Create
    {
        public string Registration { get; set; }
        public bool MotRequest { get; set; }
        public bool ServiceRequest { get; set; }
        public DateTime PreferedDate { get; set; }
        public string PreferedTime { get; set; }
        public string Message { get; set; }
    }
}
