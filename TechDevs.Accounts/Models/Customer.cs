using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace TechDevs.Accounts
{

    [BsonIgnoreExtraElements]
    [BsonDiscriminator("Customer", Required = true)]
    public class Customer : AuthUser
    {
        public CustomerData CustomerData { get; set; }

        public Customer()
        {
            CustomerData = new CustomerData();
        }
    }

    [BsonIgnoreExtraElements]
    public class CustomerData
    {
        public List<CustomerVehicle> MyVehicles { get; set; }
        public List<VehicleListing> SavedVehicles { get; set; }
        public List<ServiceHistory> ServiceHistories { get; set; }

        public CustomerData()
        {
            MyVehicles = new List<CustomerVehicle>();
            SavedVehicles = new List<VehicleListing>();
            ServiceHistories = new List<ServiceHistory>();
        }
    }

    [BsonIgnoreExtraElements]
    public class ServiceHistory
    {
        public string Registration { get; set; }
        public int Mileage { get; set; }
        public DateTime ServiceDate { get; set; }
        public string ServiceNotes { get; set; }
        public int LoyaltyPointsEarned { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CustomerVehicle
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string Registration { get; set; }
        public string Colour { get; set; }
        public string FuelType { get; set; }
        public int Year { get; set; }
        public List<MotResult> MOTResults { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class VehicleListing
    {
        public string Registration { get; set; }
        public string ListingId { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class CustomerProfile : AuthUserProfile
    {
        public CustomerData CustomerData { get; set; }
        public CustomerProfile(Customer user)
        {
            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAddress = user.EmailAddress;
            AgreedToTerms = user.AgreedToTerms;
            ValidatedEmail = user.ValidatedEmail;
            CustomerData = user.CustomerData;
        }
    }

    [BsonIgnoreExtraElements]
    public class MotResult
    {
        public string CompletedDate { get; set; }
        public string TestResult { get; set; }
        public string ExpiryDate { get; set; }
        public string OdometerValue { get; set; }
        public string OdometerUnit { get; set; }
        public string OdometerResultType { get; set; }
        public string MotTestNumber { get; set; }
        public List<MotComment> Comments { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class MotComment
    {
        public string Text { get; set; }
        public string Type { get; set; }
    }
}