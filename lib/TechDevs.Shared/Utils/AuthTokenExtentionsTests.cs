using System;
using Xunit;

namespace TechDevs.Shared.Utils
{
    public class AuthTokenExtentionsTests
    {
        private readonly string testToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjJiMDY2NmIxLWYwMGYtNDhlYS05MDA0LTg1YjQ5MzgzNzI1YyIsIkdpYnNvbi1DbGllbnRLZXkiOiJjYXJzaG9wIiwiR2lic29uLUNsaWVudElkIjoiMDkwMWNlNDYtYmM5ZS00ZGU3LWEyYmYtYjgxYjg3ZTQ3ZjYyIiwibmJmIjoxNTQ5NDAxMTk1LCJleHAiOjE1NDk0MDQ3OTUsImlhdCI6MTU0OTQwMTE5NX0.y7MqtR9NqLKzxctZTjppt2bLJP8yluX4P7VLzN_HZUs";
        private readonly string testTokenInvalidGuidClientId = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjJiMDY2NmIxLWYwMGYtNDhlYS05MDA0LTg1YjQ5MzgzNzI1YyIsIkdpYnNvbi1DbGllbnRLZXkiOiJjYXJzaG9wIiwiR2lic29uLUNsaWVudElkIjoiY2Fyc2hvcCIsIm5iZiI6MTU0OTQwMTE5NSwiZXhwIjoxNTQ5NDA0Nzk1LCJpYXQiOjE1NDk0MDExOTV9.OpFa9o4lTyu-L7hoF9cogWrtN5OXlsvcp4oT2y1mwRM";
        private readonly string testTokenInvalidGuidUserId = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNhcnNob3AiLCJHaWJzb24tQ2xpZW50S2V5IjoiY2Fyc2hvcCIsIkdpYnNvbi1DbGllbnRJZCI6ImNhcnNob3AiLCJuYmYiOjE1NDk0MDExOTUsImV4cCI6MTU0OTQwNDc5NSwiaWF0IjoxNTQ5NDAxMTk1fQ.N2JTQoaQHdGRzW8uxJxJ551gGkE_0ZqLIN8Qx4WvQlI";

        [Fact] 
        void GetUserId_Should_EqualEmptyGUID_TokenIsInvalid()
        {
            // Arrange
            string token = "THIS IS NOT A REAL JWT TOKEN";
            // Act 
            var result = token.GetUserId();
            // Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void GetUserId_Should_Return_Correct_Value()
        {
            // Arrange
            string token = testToken;
            // Act
            var result = token.GetUserId();
            // Assert
            Assert.Equal(Guid.Parse("2b0666b1-f00f-48ea-9004-85b49383725c"), result);
        }

        [Fact]
        public void GetUserId_Should_Return_TypeOf_Guid()
        {
            // Arrange
            string token = testToken;
            // Act
            var result = token.GetUserId();
            // Assert
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public void GetUserId_Should_ThrowException_NonGuidValue()
        {
            // Arrange
            string token = testTokenInvalidGuidUserId;
            // Act & Assert
            Assert.Throws<FormatException>(() => token.GetUserId());
        }

        [Fact]
        void GetClientId_Should_ThrowException_TokenIsInvalid()
        {
            // Arrange
            string token = "THIS IS NOT A REAL JWT TOKEN";
            // Act
            var result = token.GetClientId();
            // Assert
            Assert.Equal(Guid.Empty, result);

        }

        [Fact]
        public void GetClientId_Should_Return_Correct_Value()
        {
            // Arrange
            string token = testToken;
            // Act
            var result = token.GetClientId();
            // Assert
            Assert.Equal(Guid.Parse("0901ce46-bc9e-4de7-a2bf-b81b87e47f62"), result);
        }

        [Fact]
        public void GetClientId_Should_Return_TypeOf_Guid()
        {
            // Arrange
            string token = testToken;
            // Act
            var result = token.GetClientId();
            // Assert
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public void GetClientId_Should_ThrowException_NonGuidValue()
        {
            // Arrange
            string token = testTokenInvalidGuidClientId;
            // Act & Assert
            Assert.Throws<FormatException>(() => token.GetClientId());
        }

        [Fact]
        void GetClientKey_Should_ReturnNull_TokenIsInvalid()
        {
            // Arrange
            string token = "THIS IS NOT A REAL JWT TOKEN";
            // Act 
            var result = token.GetClientKey();
            // Assert
            Assert.Null(result);

        }


        [Fact]
        public void GetClientKey_Should_Return_Correct_Value()
        {
            // Arrange
            string token = testToken;
            // Act
            var result = token.GetClientKey();
            // Assert
            Assert.Equal("carshop", result);
        }

        [Fact]
        public void GetClientKey_Should_Return_TypeOf_String()
        {
            // Arrange
            string token = testToken;
            // Act
            var result = token.GetClientKey();
            // Assert
            Assert.IsType<string>(result);
        }
    }
}