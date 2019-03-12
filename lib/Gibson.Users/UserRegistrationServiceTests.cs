using System;
using System.Threading.Tasks;
using Gibson.Shared.Repositories.Tests;
using Microsoft.Extensions.Options;
using TechDevs.Shared;
using TechDevs.Shared.Models;
using Xunit;

namespace Gibson.Users
{
    public class UserRegistrationServiceTests : IClassFixture<DatabaseTestFixture>
    {
        private readonly IOptions<MongoDbSettings> _settings;

        public UserRegistrationServiceTests(DatabaseTestFixture fixture)
        {
            var connString = fixture.Db.ConnectionString;
            var dbSettings = new MongoDbSettings { ConnectionString = connString, Database = "Testing" };
            _settings = Options.Create(dbSettings);
        }

        private IUserRepository GetMockRepo() => new UserRepository("DummyUsers", _settings);
        private IPasswordHasher GetPasswordHasher() => new BCryptPasswordHasher();
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_OnEmptyClientId()
        {
            // Arrange
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration();
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RegisterUser(reg, Guid.Empty));
        }
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_WhenNotAgreedToTerms()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            const string username = "DUMMYUSER";
            var existingUser = new User { Username = username};
            await repo.Create(existingUser, clientId);
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration {EmailAddress = username, AggreedToTerms = false};
            // Act & Assert
            try
            {
                await sut.RegisterUser(reg, clientId);
                Assert.True(false, "Method should have failed but did not");
            }
            catch (UserRegistrationException regEx)
            {
                Assert.Contains(regEx.RegistrationErrors, x => x == "Must agree to terms and conditions");
            }
        }
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_WhenUsernameAlreadyRegistered()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            const string username = "DUMMYUSER";
            var existingUser = new User { Username = username};
            await repo.Create(existingUser, clientId);
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration {EmailAddress = username };
            // Act & Assert
            try
            {
                await sut.RegisterUser(reg, clientId);
                Assert.True(false, "Method should have failed but did not");
            }
            catch (UserRegistrationException regEx)
            {
                Assert.Contains(regEx.RegistrationErrors, x => x == "Username has already been registered");
            }
        }
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_WhenEmailAddressIsInvalidFormat()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            const string username = "NotAValidEmail";
            var existingUser = new User { Username = username};
            await repo.Create(existingUser, clientId);
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration {EmailAddress = username };
            // Act & Assert
            try
            {
                await sut.RegisterUser(reg, clientId);
                Assert.True(false, "Method should have failed but did not");
            }
            catch (UserRegistrationException regEx)
            {
                Assert.Contains(regEx.RegistrationErrors, x => x == "Not a valid email address");
            }
        }
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_WhenFirstNameIsNull()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            const string username = "NotAValidEmail";
            var existingUser = new User { Username = username};
            await repo.Create(existingUser, clientId);
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration {EmailAddress = username };
            // Act & Assert
            try
            {
                await sut.RegisterUser(reg, clientId);
                Assert.True(false, "Method should have failed but did not");
            }
            catch (UserRegistrationException regEx)
            {
                Assert.Contains(regEx.RegistrationErrors, x => x == "First name is required");
            }
        }
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_WhenLastNameIsNull()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var repo = GetMockRepo();
            const string username = "NotAValidEmail";
            var existingUser = new User { Username = username};
            await repo.Create(existingUser, clientId);
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration {EmailAddress = username };
            // Act & Assert
            try
            {
                await sut.RegisterUser(reg, clientId);
                Assert.True(false, "Method should have failed but did not");
            }
            catch (UserRegistrationException regEx)
            {
                Assert.Contains(regEx.RegistrationErrors, x => x == "Last name is required");
            }
        }
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_WhenAuthTypeIsUnsupported()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            const string username = "email@mail.com";
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration
            {
                EmailAddress = username,
                FirstName = "FirstName",
                LastName = "LastName",
                UserType = GibsonUserType.Customer,
                ProviderName = "NotSupported"
            };
            // Act & Assert
            try
            {
                await sut.RegisterUser(reg, clientId);
                Assert.True(false, "Method should have failed but did not");
            }
            catch (UserRegistrationException regEx)
            {
                Assert.Contains(regEx.RegistrationErrors, x => x == "Auth provider not supported");
            }
        }
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_WhenUserType_IsNotSet()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            const string username = "email@mail.com";
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration
            {
                EmailAddress = username,
                FirstName = "FirstName",
                LastName = "LastName",
                UserType = GibsonUserType.NotSet,
                ProviderName = "Gibson",
            };
            // Act & Assert
            try
            {
                await sut.RegisterUser(reg, clientId);
                Assert.True(false, "Method should have failed but did not");
            }
            catch (UserRegistrationException regEx)
            {
                Assert.Contains(regEx.RegistrationErrors, x => x == "User type is not set");
            }
        }
        
        [Fact]
        public async Task RegisterUser_Should_ThrowException_WhenEmailAddressIsNull()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            var reg = new UserRegistration();
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.RegisterUser(reg, clientId));
        }

        [Fact]
        public async Task RegisterUser_Result_ShouldHave_MatchingUser()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                Password =  "Password",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Gibson",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(reg.FirstName, result.UserProfile.FirstName);
            Assert.Equal(reg.LastName, result.UserProfile.LastName);
            Assert.Equal(reg.EmailAddress, result.UserProfile.Email);
        }
        
        [Fact]
        public async Task RegisterUser_Result_ShouldHave_UserType()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                Password =  "Password",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Google",
                ProviderId = "GoogleId",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.True(result.UserType != GibsonUserType.NotSet);
        }
        
        [Fact]
        public async Task RegisterUser_Should_Return_Matching_UserType()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                Password =  "Password",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Google",
                ProviderId = "GoogleId",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.Equal(reg.UserType, result.UserType);
        }
        
        [Fact]
        public async Task RegisterUser_Should_Return_GoogleEnum_For_GoogleProvider()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Google",
                ProviderId = "GoogleId",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.Equal(GibsonAuthProvider.Google, result.AuthProfile.AuthProvider);
        }
        
        [Fact]
        public async Task RegisterUser_Should_Return_GibsonEnum_For_GibsonProvider()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                Password =  "Password",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Gibson",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.Equal(GibsonAuthProvider.Gibson, result.AuthProfile.AuthProvider);
        }
        
        [Fact]
        public async Task RegisterUser_Should_Return_PasswordHash_For_GibsonAuth()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                Password =  "Password",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Gibson",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.NotNull(result.AuthProfile.PasswordHash);
        }

        [Fact]
        public async Task RegisterUser_Should_Return_PasswordHash_ThatCanBeValidated()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                Password =  "Password",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Gibson",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.True(GetPasswordHasher().VerifyHashedPassword(result.AuthProfile.PasswordHash, reg.Password));
        }


        [Fact]
        public async Task RegisterUser_Should_Return_ProviderId_WhenGoogleAuthSelected()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Google",
                ProviderId = "GoogleId",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.Equal(reg.ProviderId, result.AuthProfile.ProviderId);
        }
        
        [Fact]
        public async Task RegisterUser_Should_Return_EnabledUser()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var reg = new UserRegistration
            {
                AggreedToTerms =  true,
                EmailAddress =  "test@test.com",
                FirstName = "FirstName",
                LastName = "LastName",
                ProviderName = "Google",
                ProviderId = "GoogleId",
                UserType = GibsonUserType.Customer
            };
            var sut = new UserRegistrationService(GetMockRepo(), GetPasswordHasher());
            // Act
            var result = await sut.RegisterUser(reg, clientId);
            // Assert
            Assert.True(result.Enabled);
        }
    }
}