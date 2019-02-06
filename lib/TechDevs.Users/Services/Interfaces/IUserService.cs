using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Users
{
    public interface IUserService<TAuthUser> where TAuthUser : IAuthUser, new()
    {
        Task<bool> Delete(string email, string clientId);
        Task<List<TAuthUser>> GetAllUsers(string clientId);
        Task<TAuthUser> GetByEmail(string email, string clientIdOrKey);
        Task<TAuthUser> GetById(string id, string clientIdOrKey);
        Task<TAuthUser> GetByJwtToken(string token);
        Task<TAuthUser> GetByProvider(string provider, string providerId, string clientIdOrKey);
        Task<TAuthUser> RegisterUser(AuthUserRegistration userRegistration, string clientIdOrKey);
        Task RequestResetPassword(string email);
        Task ResetPassword(string email, string resetPasswordToken, string clientIdOrKey);
        Task<TAuthUser> SetPassword(string email, string password, string clientIdOrKey);
        Task<TAuthUser> UpdateEmail(string currentEmail, string newEmail, string clientIdOrKey);
        Task<TAuthUser> UpdateName(string email, string firstName, string lastName, string clientIdOrKey);
        Task<TAuthUser> UpdateContactNuber(string email, string contactNumber, string clientIdOrKey);
        Task ValidateCanRegister(AuthUserRegistration userRegistration, string clientIdOrKey);
         Task<TAuthUser> SubmitInvitation(AuthUserInvitationRequest invite, string clientIdOrKey);
        Task<TAuthUser> GetUserByInviteKey(string inviteKey, string clientIdOrKey);
        Task<TAuthUser> AcceptInvitation(AuthUserInvitationAcceptRequest req, string clientIdOrKey);
        Task SendEmailInvitation(string email, string clientIdOrKey);
        Task<TAuthUser> SetValidatedEmail(bool isValidated, string email, string clientIdOrKey);
    }
}