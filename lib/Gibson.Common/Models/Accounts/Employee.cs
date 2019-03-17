namespace Gibson.Common.Models
{
    public class Employee : AuthUser
    {
        public string Role { get; set; }
        public string JobTitle { get; set; }
        public EmployeeData EmployeeData { get; set; }
    }
}