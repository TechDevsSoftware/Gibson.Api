namespace TechDevs.Core.UserManagement
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

        string PasswordHash { get; set; }


        bool AgreedToTerms { get; set; }
        bool ValidatedEmail { get; set; }
    }
}