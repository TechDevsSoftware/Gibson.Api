//using System;
//using System.Threading.Tasks;
//using Moq;
//using TechDevs.Core.UserManagement;

//using Xunit;

//namespace TechDevs.Core.UserManagement.UnitTests
//{
//    public class UserProfileServiceUnitTests
//    {
//        [Fact]
//        public async Task GetUserByEmailShouldReturnIUser()
//        {
//            // Arrange
//            string userEmail = "stevekent55@gmail.com";
//            var repo = new Mock<IUserRepository>();
//            repo.Setup(x => x.GetUser(userEmail)).ReturnsAsync(UserStubs.User1);
//            var userProfileService = new UserProfileService(repo.Object);
//            // Act
//            var result = await userProfileService.GetUserProfile(userEmail);
//            // Assert
//            Assert.IsAssignableFrom<IUser>(result);
//            Assert.Equal(userEmail, result.EmailAddress);
//        }

//        [Fact]
//        public async Task GetUserByEmailShouldThrowExceptionWhenNotFound()
//        {
//            // Arrange
//            string userEmail = "EmailDoesNotExist@fake.com";
//            var repo = new Mock<IUserRepository>();
//            repo.Setup(x => x.GetUser(userEmail)).Returns<IUser>(null);
//            var userProfileService = new UserProfileService(repo.Object);
//            // Act & Assert
//            await Assert.ThrowsAnyAsync<Exception>(async () => await userProfileService.GetUserProfile(userEmail));
//        }

//    }
//}
