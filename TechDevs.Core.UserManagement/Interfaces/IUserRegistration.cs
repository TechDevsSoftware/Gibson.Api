using System;
namespace TechDevs.Core.UserManagement
{
    public interface IUserRegistration
    {
        // Required
        string FirstName { get; set; }
        string LastName { get; set; }
        string EmailAddress { get; set; }
        bool AggreedToTerms { get; set; }

        // Optional
		string VehicleRegistration { get; set; }
    }
}
