using System;
using System.Collections.Generic;

namespace TechDevs.Shared.Models
{
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
}
