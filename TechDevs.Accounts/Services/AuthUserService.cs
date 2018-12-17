using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechDevs.Accounts.Models;
using TechDevs.Accounts.Repositories;
using TechDevs.Mail;

namespace TechDevs.Accounts
{
    public class CustomerService : AuthUserService<Customer>
    {
        public CustomerService(IAuthUserRepository<Customer> userRepo, IPasswordHasher passwordHasher, IEmailer emailer, IOptions<AppSettings> appSettings)
            : base(userRepo, passwordHasher, emailer, appSettings)
        {
        }
    }

    public class EmployeeService : AuthUserService<Employee>
    {
        public EmployeeService(IAuthUserRepository<Employee> userRepo, IPasswordHasher passwordHasher, IEmailer emailer, IOptions<AppSettings> appSettings)
            : base(userRepo, passwordHasher, emailer, appSettings)
        {
        }
    }

    public abstract class AuthUserService : AuthUserService<AuthUser>
    {
        public AuthUserService(IAuthUserRepository<AuthUser> userRepo, IPasswordHasher passwordHasher, IEmailer emailer, IOptions<AppSettings> appSettings)
            : base(userRepo, passwordHasher, emailer, appSettings)
        {
        }

        public override Task<AuthUser> RegisterUser(AuthUserRegistration userRegistration, string clientId)
        {
            throw new Exception("Must be registered using a Customer or Employee service. Not the base AuthUser service");
        }
    }

    public abstract class AuthUserService<TAuthUser> : IAuthUserService<TAuthUser> where TAuthUser : AuthUser, new()
    {
        private readonly IAuthUserRepository<TAuthUser> _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailer _emailer;
        private readonly AppSettings _appSettings;

        public AuthUserService(IAuthUserRepository<TAuthUser> userRepo, IPasswordHasher passwordHasher, IEmailer emailer, IOptions<AppSettings> appSettings)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _emailer = emailer;
            _appSettings = appSettings.Value;
        }

        public virtual async Task<List<TAuthUser>> GetAllUsers(string clientId)
        {
            var result = await _userRepo.FindAll(clientId);
            return result;
        }

        public virtual async Task<TAuthUser> RegisterUser(AuthUserRegistration userRegistration, string clientId)
        {
            await ValidateCanRegister(userRegistration, clientId);

            var newAuthUser = new TAuthUser
            {
                Id = Guid.NewGuid().ToString(),
                ClientId = new DBRef { Id = clientId },
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                EmailAddress = userRegistration.EmailAddress,
                AgreedToTerms = userRegistration.AggreedToTerms,
                ProviderName = userRegistration.ProviderName,
                ProviderId = userRegistration.ProviderId,
                Disabled = userRegistration.IsInvite
            };

            var result = await _userRepo.Insert(newAuthUser, clientId);
            if (userRegistration.IsInvite) return result;
            if (userRegistration.ProviderName != "TechDevs" && userRegistration.Password == null) return result;
            var resultAfterPassword = await SetPassword(result.EmailAddress, userRegistration.Password, clientId);
            return resultAfterPassword;
        }

        public virtual async Task ValidateCanRegister(AuthUserRegistration userRegistration, string clientId)
        {
            var validationErrors = new StringBuilder();

            // User must have agreed to the terms
            if (!userRegistration.IsInvite && !userRegistration.AggreedToTerms)
                validationErrors.Append("Must agree to terms and conditions");

            // Email address cannot already exist
            if (await _userRepo.UserExists(userRegistration.EmailAddress, clientId))
                validationErrors.AppendLine("Email address has already been registered");

            // Email address must be valid format
            const string emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (!Regex.IsMatch(userRegistration.EmailAddress, emailRegex, RegexOptions.IgnoreCase))
                validationErrors.AppendLine("Email address is invalid");

            // Check required fields
            if (string.IsNullOrEmpty(userRegistration.FirstName)) validationErrors.AppendLine("First name is required");
            if (string.IsNullOrEmpty(userRegistration.LastName)) validationErrors.Append("Last name is required");
            if (string.IsNullOrEmpty(userRegistration.EmailAddress)) validationErrors.Append("Email address is required");

            if (validationErrors.Length > 0)
                throw new UserRegistrationException(userRegistration, validationErrors.ToString());
        }

        public virtual async Task<TAuthUser> UpdateEmail(string currentEmail, string newEmail, string clientId)
        {
            // Get user
            var user = await _userRepo.FindByEmail(currentEmail, clientId);
            if (user == null) throw new Exception("User not found");
            // Check that the new email is not already taken
            var existingEmail = await _userRepo.FindByEmail(newEmail, clientId);
            if (existingEmail != null) throw new Exception("Email address already in use");
            // Change the email
            await _userRepo.SetUsername(user, newEmail, clientId);
            var updatedUser = await _userRepo.SetEmail(user, newEmail, clientId);
            if (updatedUser == null) throw new Exception("Email update failed");
            // Set the username along with the username as we dont need a seperate username
            await _userRepo.SetUsername(user, newEmail, clientId);
            return updatedUser;
        }

        public virtual async Task<bool> Delete(string email, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            if (user == null) throw new Exception("User not found");
            return await _userRepo.Delete(user, clientId);
        }

        public virtual async Task<TAuthUser> GetByEmail(string email, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            return user;
        }

        public virtual async Task<TAuthUser> GetByProvider(string provider, string providerId, string clientId)
        {
            var user = await _userRepo.FindByProvider(provider, providerId, clientId);
            return user;
        }

        public virtual async Task<TAuthUser> SetPassword(string email, string password, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            if (user == null) throw new Exception("User not found");
            var hashedPassword = _passwordHasher.HashPassword(user, password);
            var result = await _userRepo.SetPassword(user, hashedPassword, clientId);
            return result;
        }

        public virtual Task RequestResetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public virtual Task ResetPassword(string email, string resetPasswordToken, string clientId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> ValidatePassword(string email, string password, string clientId)
        {
            try
            {
                var user = await _userRepo.FindByEmail(email, clientId);
                if (user == null) return false;
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public virtual async Task<TAuthUser> GetById(string id, string clientId)
        {
            var user = await _userRepo.FindById(id, clientId);
            return user;
        }

        public virtual async Task<TAuthUser> EnableAccount(string email, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            if (user == null) throw new Exception("User could not be found");
            var result = await _userRepo.SetDisabled(user.Id, false, clientId);
            return result;
        }

        public virtual async Task<TAuthUser> DisableAccount(string email, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            if (user == null) throw new Exception("User could not be found");
            var result = await _userRepo.SetDisabled(user.Id, true, clientId);
            return result;
        }

        public virtual async Task<TAuthUser> SetValidatedEmail(bool isValidated, string email, string clientId)
        {
            var user = await _userRepo.FindByEmail(email, clientId);
            if (user == null) throw new Exception("User could not be found");
            var result = await _userRepo.SetValidatedEmail(user.Id, isValidated, clientId);
            return result;
        }

        #region Invites

        public virtual async Task<TAuthUser> SubmitInvitation(AuthUserInvitationRequest invite, string clientId)
        {
            // Generate the Invitation
            var fakeSentByUserId = Guid.NewGuid().ToString();
            var invitationRecord = new AuthUserInvitation(invite, fakeSentByUserId);

            // Map the invite into a new user request
            var userReq = new AuthUserRegistration
            {
                IsInvite = true,
                AggreedToTerms = false,
                ProviderName = "TechDevs",
                FirstName = invite.FirstName,
                LastName = invite.LastName,
                EmailAddress = invite.Email        
            };

            // Check to see if the user already exists
            var existingUser = await GetByEmail(invitationRecord.Email, clientId);
            if (existingUser != null) throw new Exception("User is already registered");
            
            // Register the user
            var user = await RegisterUser(userReq, clientId);
            if (user == null) throw new Exception("Failed to register the user from an invitation request");

            // Build the email notification to the user
            invitationRecord.InvitationSubject = $"You have been invited to {invite.ClientName}";
            invitationRecord.InvitationBody = $"" +
                $"Hi {invite.FirstName}, {Environment.NewLine}" +
                $"You have been invited to {invite.ClientName} {Environment.NewLine}" +
                $"Follow this link to complete your registration {Environment.NewLine} {Environment.NewLine}" +
                $"{_appSettings.InvitationSiteRoot}/clients/{clientId}/employees/invite/{invitationRecord.InvitationKey}";

            // Set the invitation record and update
            var newUser = await _userRepo.SetInvitation(user.Id, invitationRecord, clientId);

            // Send the email
            await SendEmailInvitation(user.Username, clientId);

            return newUser;
        } 
        
        public virtual async Task<TAuthUser> AcceptInvitation(AuthUserInvitationAcceptRequest req, string clientId)
        {
            // Get the user
            var user = await GetByEmail(req.Email, clientId);
            if (user == null) throw new Exception("User not found");

            // Check that the invitaiton key matches the database
            if (user?.Invitation?.InvitationKey != req.InviteKey) throw new Exception("Invalid invite token provided");

            // Check that the invitation is still valid
            if (user?.Invitation?.InviteExpiry < DateTime.Now) throw new Exception("Invitation has expired. Contact your admininstrator to send a new invitation");

            // Set the password
            user = await SetPassword(user.NormalisedEmail, req.Password, clientId);

            // Activate the account
            user = await EnableAccount(user.EmailAddress, clientId);

            // Se the Validated Email flag
            user = await SetValidatedEmail(true, user.EmailAddress, clientId);

            // Set Invtation Status
            user = await _userRepo.SetInvitationStatus(user.Id, AuthUserInvitationStatus.Completed, clientId);

            return user;
        }

        public virtual async Task<TAuthUser> GetUserByInviteKey(string inviteKey, string clientId)
        {
            var result = await _userRepo.GetUserByInvitationKey(inviteKey, clientId);
            if (result == null) throw new Exception("User could not be found");
            return result;

        }
        
        public virtual async Task SendEmailInvitation(string email, string clientId)
        {
            // Get the user
            var user = await GetByEmail(email, clientId);

            // Check that the user is pending invite
            if (user.Invitation.Status.Value != "Pending")
                throw new Exception("Invalid invite status for sending email");
                       
            // Send the email
            await _emailer.SendSecurityEmail(user.EmailAddress, user.Invitation.InvitationSubject, user.Invitation.InvitationBody, false);
        }

        #endregion
    }

}