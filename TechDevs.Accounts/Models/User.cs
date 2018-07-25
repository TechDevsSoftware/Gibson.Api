using MongoDB.Bson.Serialization.Attributes;

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
    }

    [BsonDiscriminator("User")]
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
    }
}