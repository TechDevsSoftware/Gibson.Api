using System;
using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class ClientData
    {
        public List<BasicOffer> BasicOffers { get; set; }
        public List<BookingRequest> BookingRequests { get; set; }

        public ClientData()
        {
            BasicOffers = new List<BasicOffer>();
            BookingRequests = new List<BookingRequest>();
        }
    }

    public class BasicOffer
    {
        public string Id { get; set; }
        public string OfferCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageSrc { get; set; }
        public string Badge { get; set; }
        public bool Enabled { get; set; }

        public BasicOffer()
        {
            Id = Guid.NewGuid().ToString();
            Enabled = false;
        }
    }

    public class BookingRequest
    {
        public Guid Id { get; set; }
        public DBRef CustomerId { get; set; }
        public string VehicleRegistration { get; set; }
        public bool MOT { get; set; }
        public bool Service { get; set; }
        public DateTime PreferedDate { get; set; }
        public string PreferedTime { get; set; }
        public string Message { get; set; }
        public bool Confirmed { get; set; }
        public bool Cancelled { get; set; }
        public bool ConfirmationEmailSent { get; set; }

        public BookingRequest()
        {
            Id = Guid.NewGuid();
            PreferedDate = DateTime.Now.AddDays(1);
            Confirmed = false;
        }
    }
}
