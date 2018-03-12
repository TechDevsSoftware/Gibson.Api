using System;
using System.Threading.Tasks;
using Moq;
using TechDevs.Core.UserManagement.Utils;
using Xunit;

namespace TechDevs.Core.UserManagement.UnitTests
{
    public class UserServiceUnitTests
    {
        [Theory]
        [ClassData(typeof(ValidUserRegistrations))]
        public async Task RegisterUser_ShouldReturnIUser(IUserRegistration validUserRegistration)
        {
            // Arrange
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(x => x.Insert(It.IsAny<IUser>())).ReturnsAsync(UserStubs.User1);
            var passwordHasher = new Mock<IPasswordHasher>();
            passwordHasher.Setup(x => x.HashPassword(UserStubs.User1, It.IsAny<string>())).Returns(Guid.NewGuid().ToString());
            var sut = new UserService(userRepo.Object, passwordHasher.Object);
            // Act
            var result = await sut.RegisterUser(validUserRegistration);
            // Assert
            Assert.IsAssignableFrom<IUser>(result);
        }

        [Fact]
        public async Task RegisterUser_ShouldThrowCustomException_WhenEmailAlreadyExists()
        {
            // Arrange
            var userRegistration = UserRegistrationStubs.ValidUser1();
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(x => x.UserExists(userRegistration.EmailAddress)).ReturnsAsync(true);
            var sut = new UserService(userRepo.Object, null);
            // Act & Assert
            await Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterUser(userRegistration));
        }

        [Theory]
        [InlineData("InvalidEmail")]
        [InlineData("Email@Company")]
        public async Task RegisterUser_ShouldThrowException_WhenEmailIsInvalid(string invalidEmail)
        {
            // Arrange
            var createUser = UserRegistrationStubs.ValidUser1();
            createUser.EmailAddress = invalidEmail;
            var userRepo = new Mock<IUserRepository>();
            var sut = new UserService(userRepo.Object, null);
            // Act & Assert
            await Assert.ThrowsAsync<UserRegistrationException>(async () => await sut.RegisterUser(createUser));
        }

        [Theory]
        [ClassData(typeof(UsersMissingRequiredFields))]
        public async Task RegisterUser_ShouldThrowUserRegistrationException_WhenRequiredFieldIsMissing(IUserRegistration invalidUserRegistration)
        {
            // Arrange
            var sut = new UserService(new Mock<IUserRepository>().Object, new Mock<IPasswordHasher>().Object);
            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(async () => await sut.RegisterUser(invalidUserRegistration));
        }

        [Fact]
        public async Task RegisterUser_ShouldThrowUserRegistrationException_WhenNotAgreedToTerms()
        {
            // Arrange
            var userRegistration = new UserRegistration
            {
                FirstName = "Steve",
                LastName = "Kent",
                EmailAddress = "dummy@mail.com",
                AggreedToTerms = false
            };
            var sut = new UserService(new Mock<IUserRepository>().Object, new Mock<IPasswordHasher>().Object);
            // Act & Assert
            await Assert.ThrowsAnyAsync<UserRegistrationException>(async () => await sut.RegisterUser(userRegistration));
        }

        [Fact]
        public async Task RegisterUser_ShouldMakeCallToRepositoryCreateMethod()
        {
            // Arrange
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(x => x.Insert(It.IsAny<IUser>())).ReturnsAsync(UserStubs.User1);
            var sut = new UserService(userRepo.Object, new Mock<IPasswordHasher>().Object);
            // Act
            var result = await sut.RegisterUser(UserRegistrationStubs.ValidUser1());
            // Assert
            userRepo.Verify(x => x.Insert(It.IsAny<IUser>()), Times.Once());
        }
    }
}