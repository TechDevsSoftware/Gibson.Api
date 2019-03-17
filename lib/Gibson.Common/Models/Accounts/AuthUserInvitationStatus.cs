namespace Gibson.Common.Models
{
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