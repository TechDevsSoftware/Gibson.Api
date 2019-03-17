using System;

namespace Gibson.Common.Models
{
    public class BookingRequest_Create
    {
        public Guid CustomerId { get; set; }
        public string Registration { get; set; }
        public bool MotRequest { get; set; }
        public bool ServiceRequest { get; set; }
        public DateTime PreferedDate { get; set; }
        public string PreferedTime { get; set; }
        public string Message { get; set; }
    }
}
