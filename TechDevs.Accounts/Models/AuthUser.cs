using System;

namespace TechDevs.Accounts
{
    public class AuthUser
    {
        public string Id { get; set; }
        public DBRef ClientId { get; set; }
        public string Username { get; set; }
        public string NormalisedUsername { get; set; }
        public string EmailAddress { get; set; }
        public string NormalisedEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AgreedToTerms { get; set; }
        public string ValidateEmailKey { get; set; }
        public bool ValidatedEmail { get; set; }
        public string PasswordHash { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }

        public AuthUserInvitation Invitation { get; set; }
        public bool Disabled { get; set; }
    }

    public class AuthUserInvitationRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
    }

    public class AuthUserInvitation
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
        public string SentById { get; set; }
        public string InvitationKey { get; set; }
        public AuthUserInvitationStatus Status { get; set; }
        public DateTime InviteSent { get; set; }
        public DateTime InviteExpiry { get; set; }

        public AuthUserInvitation(AuthUserInvitationRequest req, string sentById)
        {
            Email = req.Email;
            FirstName = req.FirstName;
            LastName = req.LastName;
            Message = req.Message;
            InvitationKey = Guid.NewGuid().ToString();
            InviteSent = DateTime.Now;
            InviteExpiry = InviteSent.AddHours(24);
            Status = AuthUserInvitationStatus.Pending;
        }
    }

    public sealed class AuthUserInvitationStatus
    {
        public static readonly AuthUserInvitationStatus Pending = new AuthUserInvitationStatus("Pending");
        public static readonly AuthUserInvitationStatus Expired = new AuthUserInvitationStatus("Expired");
        public static readonly AuthUserInvitationStatus Completed = new AuthUserInvitationStatus("Completed");

        private AuthUserInvitationStatus(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}