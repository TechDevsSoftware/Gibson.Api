namespace Gibson.Common.Models
{
    public interface IAuthUser
    {
        bool AgreedToTerms { get; set; }
        DBRef ClientId { get; set; }
        bool Disabled { get; set; }
        string EmailAddress { get; set; }
        string FirstName { get; set; }
        string Id { get; set; }
        AuthUserInvitation Invitation { get; set; }
        string LastName { get; set; }
        string NormalisedEmail { get; set; }
        string NormalisedUsername { get; set; }
        string PasswordHash { get; set; }
        string ProviderId { get; set; }
        string ProviderName { get; set; }
        string Username { get; set; }
        bool ValidatedEmail { get; set; }
        string ValidateEmailKey { get; set; }
    }
}