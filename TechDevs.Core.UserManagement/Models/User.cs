namespace TechDevs.Core.UserManagement
{
    public class User : IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string VehicleRegistration { get; set; }
        public bool AggreedToTerms { get; set; }
    }
}