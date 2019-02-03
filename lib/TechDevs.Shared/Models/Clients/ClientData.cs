using System;
using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
    public class ClientData
    {
        public List<BasicOffer> BasicOffers { get; set; }

        public ClientData()
        {
            BasicOffers = new List<BasicOffer>();
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
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string CustomerId { get; set; }
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
        public BookingCustomer Customer { get; set; }

        public BookingRequest()
        {
            Id = Guid.NewGuid().ToString();
            PreferedDate = DateTime.UtcNow.AddDays(1);
            RequestDate = DateTime.UtcNow;
            Confirmed = false;
        }
    }

    public class BookingRequest_Create
    {
        public string Registration { get; set; }
        public bool MotRequest { get; set; }
        public bool ServiceRequest { get; set; }
        public DateTime PreferedDate { get; set; }
        public string PreferedTime { get; set; }
        public string Message { get; set; }
    }

    public class BookingCustomer
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ContactNumber { get; set; }

        public BookingCustomer()
        {
        }

        public BookingCustomer(Customer customer)
        {
            Id = customer.Id;
            ClientId = customer.ClientId.Id;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            EmailAddress = customer.EmailAddress;
            ContactNumber = customer.ContactNumber;
        }
    }
}
