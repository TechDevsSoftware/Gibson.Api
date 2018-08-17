using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public interface IAuthUserService<TAuthUser> where TAuthUser : IAuthUser, new()
    {
        Task<bool> Delete(string email, string clientId);
        Task<List<TAuthUser>> GetAllUsers(string clientId);
        Task<TAuthUser> GetByEmail(string email, string clientId);
        Task<TAuthUser> GetById(string id, string clientId);
        Task<TAuthUser> GetByProvider(string provider, string providerId, string clientId);
        Task<TAuthUser> RegisterUser(TAuthUser newUser, IAuthUserRegistration userRegistration, string clientId);
        Task RequestResetPassword(string email);
        Task ResetPassword(string email, string resetPasswordToken, string clientId);
        Task<TAuthUser> SetPassword(string email, string password, string clientId);
        Task<TAuthUser> UpdateEmail(string currentEmail, string newEmail, string clientId);
        Task ValidateCanRegister(IAuthUserRegistration userRegistration, string clientId);
        Task<bool> ValidatePassword(string email, string password, string clientId);
    }
}