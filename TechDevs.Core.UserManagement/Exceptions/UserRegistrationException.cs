using System;

namespace TechDevs.Core.UserManagement
{
    public class UserRegistrationException : Exception
    {
        public UserRegistrationException(IUserRegistration userRegistration)
        {
            UserRegistration = userRegistration;
        }
        public UserRegistrationException(IUserRegistration userRegistration, string message) : base(message)
        {
            UserRegistration = userRegistration;
        }
        public UserRegistrationException(IUserRegistration userRegistration, string message, Exception inner) : base(message, inner)
        {
            UserRegistration = userRegistration;
        }

        public IUserRegistration UserRegistration { get; }
    }
}