using System;

namespace TechDevs.Shared.Models
{
    public class AuthUserInvitation
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
        public string SentById { get; set; }
        public string InvitationKey { get; set; }
        public string ClientName { get; set; }
        public AuthUserInvitationStatus Status { get; set; }
        public DateTime InviteSent { get; set; }
        public DateTime InviteExpiry { get; set; }
        
        public string InvitationSubject { get; set; }
        public string InvitationBody { get; set; }

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
            ClientName = req.ClientName;
        }
    }
}