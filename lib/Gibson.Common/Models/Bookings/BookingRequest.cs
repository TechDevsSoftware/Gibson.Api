using System;

namespace Gibson.Common.Models
{
    public class BookingRequest : CustomerEntity
    {
        public string Registration { get; set; }
        public bool MOT { get; set; }
        public bool Service { get; set; }
        public DateTime PreferedDate { get; set; }
        public string PreferedTime { get; set; }
        public string Message { get; set; }
        public bool Confirmed { get; set; }
        public bool Cancelled { get; set; }
        public bool ConfirmationEmailSent { get; set; }
        public DateTime RequestDate { get; set; }
        public CustomerVehicle Vehicle { get; set; }
        public PublicUser Customer { get; set; }

        public BookingRequest()
        {
            PreferedDate = DateTime.UtcNow.AddDays(1);
            RequestDate = DateTime.UtcNow;
            Confirmed = false;
        }
    }
}
