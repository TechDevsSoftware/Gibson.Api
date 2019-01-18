using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Users
{
    public interface IAuthUserService<TAuthUser> where TAuthUser : IAuthUser, new()
    {
        Task<bool> Delete(string email, string clientId);
        Task<List<TAuthUser>> GetAllUsers(string clientId);
        Task<TAuthUser> GetByEmail(string email, string clientId);
        Task<TAuthUser> GetById(string id, string clientId);
        Task<TAuthUser> GetByProvider(string provider, string providerId, string clientId);
        Task<TAuthUser> RegisterUser(AuthUserRegistration userRegistration, string clientId);
        Task RequestResetPassword(string email);
        Task ResetPassword(string email, string resetPasswordToken, string clientId);
        Task<TAuthUser> SetPassword(string email, string password, string clientId);
        Task<TAuthUser> UpdateEmail(string currentEmail, string newEmail, string clientId);
        Task<TAuthUser> UpdateName(string email, string firstName, string lastName, string clientId);
        Task<TAuthUser> UpdateContactNuber(string email, string contactNumber, string clientId);
        Task ValidateCanRegister(AuthUserRegistration userRegistration, string clientId);
        Task<bool> ValidatePassword(string email, string password, string clientId);
        Task<TAuthUser> SubmitInvitation(AuthUserInvitationRequest invite, string clientId);
        Task<TAuthUser> GetUserByInviteKey(string inviteKey, string clientId);
        Task<TAuthUser> AcceptInvitation(AuthUserInvitationAcceptRequest req, string clientId);
        Task SendEmailInvitation(string email, string clientId);
        Task<TAuthUser> SetValidatedEmail(bool isValidated, string email, string clientId);
    }
}