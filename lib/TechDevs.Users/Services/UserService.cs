using Gibson.AuthTokens;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechDevs.Clients;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;

namespace TechDevs.Users
{
    public class UserService : UserService<AuthUser>
    {
        public UserService(
            IAuthUserRepository<AuthUser> userRepo,
            IPasswordHasher passwordHasher,
            IOptions<AppSettings> appSettings,
            IClientService clientService,
            IAuthTokenService tokenService)
            : base(userRepo, passwordHasher, appSettings, clientService, tokenService)
        {
        }

        public override Task<AuthUser> RegisterUser(UserRegistration userRegistration, string clientId)
        {
            throw new Exception("Must be registered using a Customer or Employee service. Not the base AuthUser service");
        }
    }

    public abstract class UserService<TAuthUser> : IUserService<TAuthUser> where TAuthUser : AuthUser, new()
    {
        public readonly IAuthUserRepository<TAuthUser> _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IClientService _clientService;
        private readonly IAuthTokenService tokenService;
        private readonly AppSettings _appSettings;

        public UserService(
            IAuthUserRepository<TAuthUser> userRepo,
            IPasswordHasher passwordHasher,
            IOptions<AppSettings> appSettings,
            IClientService clientService,
            IAuthTokenService tokenService)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _clientService = clientService;
            this.tokenService = tokenService;
            _appSettings = appSettings.Value;
        }


        public virtual async Task<List<TAuthUser>> GetAllUsers(string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var result = await _userRepo.FindAll(client.Id);
            return result;
        }

        public virtual async Task<TAuthUser> RegisterUser(UserRegistration userRegistration, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);

            await ValidateCanRegister(userRegistration, client.Id);

            var newAuthUser = new TAuthUser
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = new DBRef { Id = client.Id },
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                EmailAddress = userRegistration.EmailAddress,
                AgreedToTerms = userRegistration.AggreedToTerms,
                ProviderName = userRegistration.ProviderName,
                ProviderId = userRegistration.ProviderId,
                Disabled = userRegistration.IsInvite
            };

            var result = await _userRepo.Insert(newAuthUser, client.Id);
            if (userRegistration.IsInvite) return result;
            if (userRegistration.ProviderName != "TechDevs" && userRegistration.Password == null) return result;
            var resultAfterPassword = await SetPassword(result.EmailAddress, userRegistration.Password, client.Id);
            return resultAfterPassword;
        }

        public virtual async Task ValidateCanRegister(UserRegistration userRegistration, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var validationErrors = new List<string>();

            // User must have agreed to the terms
            if (!userRegistration.IsInvite && !userRegistration.AggreedToTerms)
                validationErrors.Add("Must agree to terms and conditions");

            // Email address cannot already exist
            if (await _userRepo.UserExists(userRegistration.EmailAddress, client.Id))
                validationErrors.Add("Email address has already been registered");

            // Email address must be valid format
            const string emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (!Regex.IsMatch(userRegistration.EmailAddress, emailRegex, RegexOptions.IgnoreCase))
                validationErrors.Add("Email address is invalid");

            // Check required fields
            if (string.IsNullOrEmpty(userRegistration.FirstName)) validationErrors.Add("First name is required");
            if (string.IsNullOrEmpty(userRegistration.LastName)) validationErrors.Add("Last name is required");
            if (string.IsNullOrEmpty(userRegistration.EmailAddress)) validationErrors.Add("Email address is required");

            if (validationErrors.Count > 0)
                throw new UserRegistrationException(userRegistration, validationErrors, "Registration validation failed");
        }

        public virtual async Task<TAuthUser> UpdateEmail(string currentEmail, string newEmail, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            // Get user
            var user = await _userRepo.FindByEmail(currentEmail, client.Id);
            if (user == null) throw new Exception("User not found");
            // Check that the new email is not already taken
            var existingEmail = await _userRepo.FindByEmail(newEmail, client.Id);
            if (existingEmail != null) throw new Exception("Email address already in use");
            // Change the email
            await _userRepo.SetUsername(user, newEmail, client.Id);
            var updatedUser = await _userRepo.SetEmail(user, newEmail, client.Id);
            if (updatedUser == null) throw new Exception("Email update failed");
            // Set the username along with the username as we dont need a seperate username
            await _userRepo.SetUsername(user, newEmail, client.Id);
            return updatedUser;
        }

        public virtual async Task<TAuthUser> UpdateName(string email, string firstName, string lastName, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            // Get User
            var user = await _userRepo.FindByEmail(email, client.Id);
            if (user == null) throw new Exception("User not found");

            var result = (firstName != user.FirstName) ? await _userRepo.UpdateUser("FirstName", firstName, user.Id, client.Id) : user;
            result = (lastName != user.LastName) ? await _userRepo.UpdateUser("LastName", lastName, user.Id, client.Id) : result;

            return result;
        }

        public virtual async Task<TAuthUser> UpdateContactNuber(string email, string contactNumber, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            // Get User
            var user = await _userRepo.FindByEmail(email, client.Id);
            if (user == null) throw new Exception("User not found");
            var result = await _userRepo.UpdateUser("ContactNumber", contactNumber, user.Id, client.Id);
            return result;
        }


        public virtual async Task<bool> Delete(string email, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var user = await _userRepo.FindByEmail(email, client.Id);
            if (user == null) throw new Exception("User not found");
            return await _userRepo.Delete(user, client.Id);
        }

        public virtual async Task<TAuthUser> GetByEmail(string email, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var user = await _userRepo.FindByEmail(email, client.Id);
            return user;
        }

        public virtual async Task<TAuthUser> GetByProvider(string provider, string providerId, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var user = await _userRepo.FindByProvider(provider, providerId, client.Id);
            return user;
        }

        public virtual async Task<TAuthUser> SetPassword(string email, string password, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var user = await _userRepo.FindByEmail(email, client.Id);
            if (user == null) throw new Exception("User not found");
            var hashedPassword = _passwordHasher.HashPassword(password);
            var result = await _userRepo.SetPassword(user, hashedPassword, client.Id);
            return result;
        }

        public virtual Task RequestResetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public virtual Task ResetPassword(string email, string resetPasswordToken, string clientIdOrKey)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<TAuthUser> GetById(string id, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var user = await _userRepo.FindById(id, client.Id);
            return user;
        }

        public virtual async Task<TAuthUser> EnableAccount(string email, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var user = await _userRepo.FindByEmail(email, client.Id);
            if (user == null) throw new Exception("User could not be found");
            var result = await _userRepo.SetDisabled(user.Id, false, client.Id);
            return result;
        }

        public virtual async Task<TAuthUser> DisableAccount(string email, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var user = await _userRepo.FindByEmail(email, client.Id);
            if (user == null) throw new Exception("User could not be found");
            var result = await _userRepo.SetDisabled(user.Id, true, client.Id);
            return result;
        }

        public virtual async Task<TAuthUser> SetValidatedEmail(bool isValidated, string email, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var user = await _userRepo.FindByEmail(email, client.Id);
            if (user == null) throw new Exception("User could not be found");
            var result = await _userRepo.SetValidatedEmail(user.Id, isValidated, client.Id);
            return result;
        }

        #region Invites

        public virtual async Task<TAuthUser> SubmitInvitation(AuthUserInvitationRequest invite, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            // Generate the Invitation
            var fakeSentByUserId = Guid.NewGuid().ToString();
            var invitationRecord = new AuthUserInvitation(invite, fakeSentByUserId);

            // Map the invite into a new user request
            var userReq = new UserRegistration
            {
                IsInvite = true,
                AggreedToTerms = false,
                ProviderName = "TechDevs",
                FirstName = invite.FirstName,
                LastName = invite.LastName,
                EmailAddress = invite.Email
            };

            // Check to see if the user already exists
            var existingUser = await GetByEmail(invitationRecord.Email, client.Id);
            if (existingUser != null) throw new Exception("User is already registered");

            // Register the user
            var user = await RegisterUser(userReq, client.Id);
            if (user == null) throw new Exception("Failed to register the user from an invitation request");

            // Build the email notification to the user
            invitationRecord.InvitationSubject = $"You have been invited to {invite.ClientName}";
            invitationRecord.InvitationBody = $"" +
                $"Hi {invite.FirstName}, {Environment.NewLine}" +
                $"You have been invited to {invite.ClientName} {Environment.NewLine}" +
                $"Follow this link to complete your registration {Environment.NewLine} {Environment.NewLine}" +
                $"{_appSettings.InvitationSiteRoot}/clients/{client.Id}/employees/invite/{invitationRecord.InvitationKey}";

            // Set the invitation record and update
            var newUser = await _userRepo.SetInvitation(user.Id, invitationRecord, client.Id);

            // Send the email
            await SendEmailInvitation(user.Username, client.Id);

            return newUser;
        }

        public virtual async Task<TAuthUser> AcceptInvitation(AuthUserInvitationAcceptRequest req, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            // Get the user
            var user = await GetByEmail(req.Email, client.Id);
            if (user == null) throw new Exception("User not found");

            // Check that the invitaiton key matches the database
            if (user?.Invitation?.InvitationKey != req.InviteKey) throw new Exception("Invalid invite token provided");

            // Check that the invitation is still valid
            if (user?.Invitation?.InviteExpiry < DateTime.Now) throw new Exception("Invitation has expired. Contact your admininstrator to send a new invitation");

            // Set the password
            user = await SetPassword(user.NormalisedEmail, req.Password, client.Id);

            // Activate the account
            user = await EnableAccount(user.EmailAddress, client.Id);

            // Se the Validated Email flag
            user = await SetValidatedEmail(true, user.EmailAddress, client.Id);

            // Set Invtation Status
            user = await _userRepo.SetInvitationStatus(user.Id, AuthUserInvitationStatus.Completed, client.Id);

            return user;
        }

        public virtual async Task<TAuthUser> GetUserByInviteKey(string inviteKey, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            var result = await _userRepo.GetUserByInvitationKey(inviteKey, client.Id);
            if (result == null) throw new Exception("User could not be found");
            return result;

        }

        public virtual async Task SendEmailInvitation(string email, string clientIdOrKey)
        {
            var client = await _clientService.GetClientIdentifier(clientIdOrKey);
            // Get the user
            var user = await GetByEmail(email, client.Id);

            // Check that the user is pending invite
            if (user.Invitation.Status.Value != "Pending")
                throw new Exception("Invalid invite status for sending email");

            // Send the email
            //await _emailer.SendSecurityEmail(user.EmailAddress, user.Invitation.InvitationSubject, user.Invitation.InvitationBody, false);
        }

        public async Task<TAuthUser> GetByJwtToken(string token)
        {
            // Extract the user id from the jwt token
            var userId = token.GetUserId().ToString();
            var clientId = token.GetClientId().ToString();
            if (string.IsNullOrEmpty(userId)) throw new Exception("Jwt Token did not contain a valid userId");

            var user = await GetById(userId, clientId);
            return user;
        }

        #endregion


    }

}