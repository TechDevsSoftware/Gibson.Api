using System;
using System.Collections;
using System.Collections.Generic;

namespace TechDevs.Accounts.Tests
{
    public static class UserStubs
    {
        public static ICustomer User1 => new Customer { FirstName = "Steve", LastName = "Kent", EmailAddress = "stevekent55@gmail.com" };
        public static ICustomer User2 => new Customer { FirstName = "Adam", LastName = "Fox", EmailAddress = "amobilefox@gmail.com" };
    }

    public static class UserRegistrationStubs
    {
        public static IAuthUserRegistration ValidUser1() => new AuthUserRegistration { FirstName = "Steve", LastName = "Kent", EmailAddress = "stevekent55@gmail.com", AggreedToTerms = true };
        public static IAuthUserRegistration ValidUser2() => new AuthUserRegistration { FirstName = "Adam", LastName = "Fox", EmailAddress = "amobilefox@gmail.com", AggreedToTerms = true };
        public static IAuthUserRegistration UserMissingFirstName => new AuthUserRegistration { LastName = "Kent", EmailAddress = "stevekent55@gmail.com" };
        public static IAuthUserRegistration UserMissingLastName => new AuthUserRegistration { FirstName = "Steve", EmailAddress = "stevekent55@gmail.com" };
        public static IAuthUserRegistration UserMissingEmailAddress => new AuthUserRegistration { FirstName = "Steve", LastName = "Kent" };
    }

    public class ValidUserRegistrations : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new Object[] { UserRegistrationStubs.ValidUser1() };
            yield return new Object[] { UserRegistrationStubs.ValidUser2() };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class UsersMissingRequiredFields : IEnumerable<object[]>
    {
        const string VALID_EMAIL = "stevekent55@gmail.com";

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new Object[] { UserRegistrationStubs.UserMissingFirstName };
            yield return new Object[] { UserRegistrationStubs.UserMissingLastName };
            yield return new Object[] { UserRegistrationStubs.UserMissingEmailAddress };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}