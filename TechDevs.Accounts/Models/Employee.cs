using MongoDB.Bson.Serialization.Attributes;

namespace TechDevs.Accounts
{
    [BsonIgnoreExtraElements]
    public class Employee : AuthUser, IEmployee
    {
        public string Role { get; set; }
        public string JobTitle { get; set; }
    }

    public class EmployeeData
    {
        public string EmployeeId { get; set; }
    }
}