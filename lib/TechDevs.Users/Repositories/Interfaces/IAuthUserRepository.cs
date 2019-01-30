using System.Collections.Generic;
using System.Threading.Tasks;
using TechDevs.Shared.Models;
using TechDevs.Shared.Models.Shared;

namespace TechDevs.Users
{
    public interface IAuthUserRepository<TAuthUser> where TAuthUser : AuthUser
    {
        Task<bool> Delete(TAuthUser user, string clientId);
        Task<TAuthUser> FindByEmail(string email, string clientId);
        Task<TAuthUser> FindById(string id, string clientId);
        Task<TAuthUser> FindByProvider(string provider, string providerId, string clientId);
        Task<List<TAuthUser>> FindAll(string clientId);
        Task<TAuthUser> Insert(TAuthUser user, string clientId);
        Task<TAuthUser> SetEmail(TAuthUser user, string email, string clientId);
        Task<TAuthUser> SetName(TAuthUser user, string firstName, string lastName, string clientId);
        Task<TAuthUser> SetPassword(TAuthUser user, string passwordHash, string clientId);
        Task<TAuthUser> SetUsername(TAuthUser user, string username, string clientId);
        Task<TAuthUser> UpdateUser<Type>(string propertyPath, Type data, string id, string clientId);
        Task<TAuthUser> UpdateUser<Type>(string propertyPath, List<Type> data, string id, string clientId);
        Task<bool> UserExists(string email, string clientId);
        Task<TAuthUser> SetInvitation(string userId, AuthUserInvitation invite, string clientId);
        Task<TAuthUser> SetDisabled(string userId, bool disabled, string clientId);
        Task<TAuthUser> GetUserByInvitationKey(string invitationKey, string clientId);
        Task<TAuthUser> SetInvitationStatus(string userId, AuthUserInvitationStatus status, string clientId);
        Task<TAuthUser> SetValidatedEmail(string userId, bool validated, string clientId);
    }
}