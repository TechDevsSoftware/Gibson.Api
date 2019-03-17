using System;
using System.Collections.Generic;
using Gibson.Common.Models;

namespace Gibson.Common.Models
{
    public class User : Entity
    {
        public GibsonUserType UserType { get; set; }
        public string Username { get; set; }
        public UserProfile UserProfile { get; set; }
        public AuthProfile AuthProfile { get; set; }
        public bool Enabled { get; set; }

        public User()
        {
            UserProfile = new UserProfile();
            AuthProfile = new AuthProfile();
        }
    }

    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
    }

    public class AuthProfile
    {
        public GibsonAuthProvider AuthProvider { get; set; }
        public string PasswordHash { get; set; }
        public string ProviderId { get; set; }
        public List<AuthEvent> AuthEvents { get; set; }

        public AuthProfile()
        {
            AuthEvents = new List<AuthEvent>();
        }
    }

    public class AuthEvent
    {
        public GibsonAuthEventType EventType { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public enum GibsonAuthEventType
    {
        SuccessfulLogin,
        FailedLogin,
        AccountDisabled,
        AccountEnabled,
        AccountCreated,
        PasswordChanged,
        PasswordResetSent,
        PasswordReset,
        InvitationSent,
        InvitationAccepted
    }

    public enum GibsonUserType
    {
        NotSet, Customer, Employee
    }

    public enum GibsonAuthProvider
    {
        Gibson, Google
    }
}
