namespace TechDevs.Shared.Models
{
    public class AuthUserInvitationRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
        public string ClientName { get; set; }
    }
}