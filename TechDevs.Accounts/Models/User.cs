using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace TechDevs.Accounts
{

    public interface IUser
    {
        string Id { get; set; }
        string Username { get; set; }
        string NormalisedUsername { get; set; }
        string EmailAddress { get; set; }
        string NormalisedEmail { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        bool AgreedToTerms { get; set; }
        bool ValidatedEmail { get; set; }
        string PasswordHash { get; set; }
        string ProviderName { get; set; }
        string ProviderId { get; set; }
        UserData UserData { get; set; }
    }

    [BsonDiscriminator("User")]
    [BsonIgnoreExtraElements]
    public class User : IUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string NormalisedUsername { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string NormalisedEmail { get; set; }
        public bool AgreedToTerms { get; set; }
        public bool ValidatedEmail { get; set; }
        public string PasswordHash { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public UserData UserData { get; set; }

        public User()
        {
            UserData = new UserData();
        }
    }

    public class UserData : IUserData
    {
        public List<UserVehicle> MyVehicles { get; set; }
        public List<VehicleListing> SavedVehicles { get; set; }
        public List<ServiceHistory> ServiceHistories { get; set; }

        public UserData()
        {
            MyVehicles = new List<UserVehicle>();
            SavedVehicles = new List<VehicleListing>();
            ServiceHistories = new List<ServiceHistory>();
        }
    }

    public class ServiceHistory
    {
        public string Registration { get; set; }
        public int Mileage { get; set; }
        public DateTime ServiceDate { get; set; }
        public string ServiceNotes { get; set; }
        public int LoyaltyPointsEarned { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class UserVehicle
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string Registration { get; set; }
        public string Colour { get; set; }
        public string FuelType { get; set; }
        public int Year { get; set; }
        public List<MotResult> MOTResults { get; set; }
    }

    public class VehicleListing
    {
        public string Registration { get; set; }
        public string ListingId { get; set; }
    }

    public class UserProfile
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AgreedToTerms { get; set; }
        public bool ValidatedEmail { get; set; }
        public UserData UserData { get; set; }

        public UserProfile(IUser user)
        {
            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAddress = user.EmailAddress;
            AgreedToTerms = user.AgreedToTerms;
            ValidatedEmail = user.ValidatedEmail;
            UserData = user.UserData;
        }
    }

    public class MotResult
    {
        public string CompletedDate { get; set; }
        public string TestResult { get; set; }
        public string ExpiryDate { get; set; }
        public string OdometerValue { get; set; }
        public string OdometerUnit { get; set; }
        public string OdometerResultType { get; set; }
        public string MotTestNumber { get; set; }
        public List<object> RfrAndComments { get; set; }
    }
}