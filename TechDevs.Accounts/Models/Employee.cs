using MongoDB.Bson.Serialization.Attributes;

namespace TechDevs.Accounts
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("Employee", Required = true)]
    public class Employee : AuthUser
    {
        public string Role { get; set; }
        public string JobTitle { get; set; }
        public EmployeeData EmployeeData { get; set; }
    }

    public class EmployeeData
    {
        public string EmployeeId { get; set; }
    }

    public abstract class AuthUserProfile
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool AgreedToTerms { get; set; }
        public bool ValidatedEmail { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class EmployeeProfile : AuthUserProfile
    {        
        public EmployeeData EmployeeData { get; set; }
        public EmployeeProfile(Employee emp)
        {
            Username = emp.Username;
            FirstName = emp.FirstName;
            LastName = emp.LastName;
            EmailAddress = emp.EmailAddress;
            AgreedToTerms = emp.AgreedToTerms;
            ValidatedEmail = emp.ValidatedEmail;
            EmployeeData = emp.EmployeeData;
        }
    }
}