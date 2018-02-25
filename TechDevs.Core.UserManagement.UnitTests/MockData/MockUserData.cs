using System;
using System.Collections;
using System.Collections.Generic;
using TechDevs.Core.UserManagement;

namespace TechDevs.Core.UserManagement.UnitTests
{
    public static class UserStubs
    {
        public static IUser User1() => new User { FirstName = "Steve", LastName = "Kent", EmailAddress = "stevekent55@gmail.com" };
        public static IUser User2() => new User { FirstName = "Adam", LastName = "Fox", EmailAddress = "amobilefox@gmail.com" };
    }

    public static class UserRegistrationStubs
    {
        public static IUserRegistration ValidUser1() => new UserRegistration { FirstName = "Steve", LastName = "Kent", EmailAddress = "stevekent55@gmail.com", AggreedToTerms = true };
        public static IUserRegistration ValidUser2() => new UserRegistration { FirstName = "Adam", LastName = "Fox", EmailAddress = "amobilefox@gmail.com", AggreedToTerms = true };
        public static IUserRegistration UserMissingFirstName => new UserRegistration { LastName = "Kent", EmailAddress = "stevekent55@gmail.com" };
        public static IUserRegistration UserMissingLastName => new UserRegistration { FirstName = "Steve", EmailAddress = "stevekent55@gmail.com" };
        public static IUserRegistration UserMissingEmailAddress => new UserRegistration { FirstName = "Steve", LastName = "Kent" };
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