using System;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts
{
    public class UserRegistrationException : Exception
    {
        public UserRegistrationException(AuthUserRegistration userRegistration)
        {
            UserRegistration = userRegistration;
        }
        public UserRegistrationException(AuthUserRegistration userRegistration, string message) : base(message)
        {
            UserRegistration = userRegistration;
        }
        public UserRegistrationException(AuthUserRegistration userRegistration, string message, Exception inner) : base(message, inner)
        {
            UserRegistration = userRegistration;
        }

        public AuthUserRegistration UserRegistration { get; }
    }
}