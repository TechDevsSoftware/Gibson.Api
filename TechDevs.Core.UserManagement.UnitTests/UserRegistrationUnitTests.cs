//using System;
//using System.Threading.Tasks;
//using Moq;
//using TechDevs.Core.UserManagement;
//using Xunit;

//namespace TechDevs.Core.UserManagement.UnitTests
//{
//    public class UserRegistrationUnitTests
//    {
//        [Theory]
//        [ClassData(typeof(ValidUserRegistrations))]
//        public async Task RegisterUser_ShouldReturnIUser(IUserRegistration validUserRegistration)
//        {
//            // Arrange
//            var userRepo = new Mock<IUserRepository>();
//            userRepo.Setup(x => x.CreateUser(It.IsAny<IUserRegistration>())).ReturnsAsync(UserStubs.User1());
//            var registrationService = new UserRegistrationService(userRepo.Object);
//            // Act
//            var result = await registrationService.RegisterUser(validUserRegistration);
//            // Assert
//            Assert.IsAssignableFrom<IUser>(result);
//        }

//        [Fact]
//        public async Task RegisterUser_ShouldThrowException_WhenEmailAlreadyExists()
//        {
//            // Arrange
//            var userRegistration = UserRegistrationStubs.ValidUser1();
//            var userRepo = new Mock<IUserRepository>();
//            userRepo
//                .Setup(x => x.EmailAlreadyRegistered(userRegistration.EmailAddress))
//                .ReturnsAsync(true);
//            var registrationService = new UserRegistrationService(userRepo.Object);
//            // Act & Assert
//            await Assert.ThrowsAsync<UserRegistrationException>(async () => await registrationService.RegisterUser(userRegistration));
//        }

//        [Theory]
//        [InlineData("InvalidEmail")]
//        [InlineData("Email@Company")]
//        public async Task RegisterUser_ShouldThrowException_WhenEmailIsInvalid(string invalidEmail)
//        {
//            // Arrange
//            var createUser = UserRegistrationStubs.ValidUser1();
//            createUser.EmailAddress = invalidEmail;
//            var userRepo = new Mock<IUserRepository>();
//            var registrationService = new UserRegistrationService(userRepo.Object);
//            // Act & Assert
//            await Assert.ThrowsAsync<UserRegistrationException>(async () => await registrationService.RegisterUser(createUser));
//        }

//        [Theory]
//        [ClassData(typeof(UsersMissingRequiredFields))]
//        public async Task RegisterUser_ShouldThrowException_WhenRequiredFieldIsMissing(IUserRegistration invalidUserRegistration)
//        {
//            // Arrange
//            var userRepo = new Mock<IUserRepository>();
//            var registrationService = new UserRegistrationService(userRepo.Object);
//            // Act & Assert
//            await Assert.ThrowsAnyAsync<Exception>(async () => await registrationService.RegisterUser(invalidUserRegistration));
//        }

//        [Fact]
//        public async Task RegisterUser_ShouldThrowException_WhenNotAgreedToTerms()
//        {
//            // Arrange
//            var userRegistration = new UserRegistration
//            {
//                FirstName = "Steve",
//                LastName = "Kent",
//                EmailAddress = "dummy@mail.com",
//                AggreedToTerms = false
//            };
//            var userRepo = new Mock<IUserRepository>();
//            var registrationService = new UserRegistrationService(userRepo.Object);
//            // Act & Assert
//            await Assert.ThrowsAnyAsync<Exception>(async () => await registrationService.RegisterUser(userRegistration));
//        }

//        [Fact]
//        public async Task RegisterUser_ShouldMakeCallToRepositoryCreateMethod()
//        {
//            // Arrange
//            var userRepo = new Mock<IUserRepository>();
//            userRepo.Setup(x => x.CreateUser(It.IsAny<IUserRegistration>())).ReturnsAsync(UserStubs.User1());
//            var registrationService = new UserRegistrationService(userRepo.Object);
//            // Act
//            var result = await registrationService.RegisterUser(UserRegistrationStubs.ValidUser1());
//            // Assert
//            userRepo.Verify(x => x.CreateUser(It.IsAny<IUserRegistration>()), Times.Once());
//        }
//    }
//}