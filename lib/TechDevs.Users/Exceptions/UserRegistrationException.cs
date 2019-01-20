using System;
using System.Collections.Generic;
using TechDevs.Shared.Models;

namespace TechDevs.Users
{
    public class UserRegistrationException : Exception
    {
        public List<string> RegistrationErrors { get; set; }

        public UserRegistrationException(AuthUserRegistration userRegistration)
        {
            UserRegistration = userRegistration;
        }
        public UserRegistrationException(AuthUserRegistration userRegistration, List<string> errors, string message) : base(message)
        {
            UserRegistration = userRegistration;
            RegistrationErrors = errors;
        }
        public UserRegistrationException(AuthUserRegistration userRegistration, List<string> errors, string message, Exception inner) : base(message, inner)
        {
            UserRegistration = userRegistration;
            RegistrationErrors = errors;
        }

        public AuthUserRegistration UserRegistration { get; }
    }
}