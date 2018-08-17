using System;

namespace TechDevs.Accounts
{
    public class UserRegistrationException : Exception
    {
        public UserRegistrationException(IAuthUserRegistration userRegistration)
        {
            UserRegistration = userRegistration;
        }
        public UserRegistrationException(IAuthUserRegistration userRegistration, string message) : base(message)
        {
            UserRegistration = userRegistration;
        }
        public UserRegistrationException(IAuthUserRegistration userRegistration, string message, Exception inner) : base(message, inner)
        {
            UserRegistration = userRegistration;
        }

        public IAuthUserRegistration UserRegistration { get; }
    }
}