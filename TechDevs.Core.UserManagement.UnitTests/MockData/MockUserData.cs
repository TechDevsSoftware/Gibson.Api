using System;
using System.Collections;
using System.Collections.Generic;

namespace TechDevs.Core.UserManagement.UnitTests
{
    public static class UserStubs
    {
        public static IUserRegistration ValidUser1() => new UserRegistration { FirstName = "Steve", LastName = "Kent", EmailAddress = "stevekent55@gmail.com" };
        public static IUserRegistration ValidUser2() => new UserRegistration { FirstName = "Adam", LastName = "Fox", EmailAddress = "amobilefox@gmail.com" };
        public static IUserRegistration UserMissingFirstName => new UserRegistration { LastName = "Kent", EmailAddress = "stevekent55@gmail.com" };
        public static IUserRegistration UserMissingLastName => new UserRegistration { FirstName = "Steve", EmailAddress = "stevekent55@gmail.com" };
        public static IUserRegistration UserMissingEmailAddress => new UserRegistration { FirstName = "Steve", LastName = "Kent" };
    }

	public class ValidUsers : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator()
		{
			yield return new Object[] { UserStubs.ValidUser1() };
			yield return new Object[] { UserStubs.ValidUser2() };
		}
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

    public class UsersMissingRequiredFields : IEnumerable<object[]>
    {
        const string VALID_EMAIL = "stevekent55@gmail.com";

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new Object[] { UserStubs.UserMissingFirstName };
            yield return new Object[] { UserStubs.UserMissingLastName };
            yield return new Object[] { UserStubs.UserMissingEmailAddress };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}