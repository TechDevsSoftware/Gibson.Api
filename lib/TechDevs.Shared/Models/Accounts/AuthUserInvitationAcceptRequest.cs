namespace TechDevs.Shared.Models
{
    public class AuthUserInvitationAcceptRequest
    {
        public string Email { get; set; }
        public string InviteKey { get; set; }
        public string Password { get; set; }
    }
}