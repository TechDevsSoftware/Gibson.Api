using System;
using Moq;
using Xunit;

namespace TechDevs.Core.UserManagement.UnitTests
{

    public class UserRegistrationUnitTests
    {
        [Theory]
        [ClassData(typeof(ValidUsers))]
        public void RegisterUserShouldReturnIUser(IUserRegistration validUserRegistration)
        {
            // Arrange
            var userRepo = new Mock<IUserRepository>();
            var registrationService = new UserRegistrationService(userRepo.Object);
            // Act
            var result = registrationService.RegisterUser(validUserRegistration);
            // Assert
            Assert.IsAssignableFrom<IUser>(result);
        }

        [Fact]
        public void RegisterUserShouldThrowExceptionWhenEmailAlreadyExists()
        {
            // Arrange
            var userRegistration = UserStubs.ValidUser1();
            var userRepo = new Mock<IUserRepository>();
            userRepo
                .Setup(x => x.UserByEmail(userRegistration.EmailAddress))
                .Returns(new User { EmailAddress = userRegistration.EmailAddress });
            var registrationService = new UserRegistrationService(userRepo.Object);
            // Act & Assert
            Assert.Throws<UserRegistrationException>(() => registrationService.RegisterUser(userRegistration));
        }

        [Theory]
        [InlineData("InvalidEmail")]
        [InlineData("Email@Company")]
        public void RegisterUserShouldThrowExceptionWhenEmailIsInvalid(string invalidEmail)
        {
            // Arrange
            var createUser = UserStubs.ValidUser1();
            createUser.EmailAddress = invalidEmail;
            var userRepo = new Mock<IUserRepository>();
            var registrationService = new UserRegistrationService(userRepo.Object);
            // Act & Assert
            Assert.Throws<UserRegistrationException>(() => registrationService.RegisterUser(createUser));
        }

        [Theory]
        [ClassData(typeof(UsersMissingRequiredFields))]
        public void RegisterUserShouldThrowExceptionWhenRequiredFieldIsMissing(IUserRegistration invalidUserRegistration)
        {
            // Arrange
            var userRepo = new Mock<IUserRepository>();
            var registrationService = new UserRegistrationService(userRepo.Object);
            // Act & Assert
            Assert.ThrowsAny<Exception>(() => registrationService.RegisterUser(invalidUserRegistration));
        }
    }
}