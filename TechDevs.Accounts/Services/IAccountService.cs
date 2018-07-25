using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public interface IAccountService
    {
        Task<List<IUser>> GetAllUsers();
        Task<IUser> RegisterUser(IUserRegistration registration);
        Task<IUser> GetByEmail(string email);
        Task<IUser> GetByProvider(string provider, string providerId);
        Task<IUser> GetById(string id);
        Task<IUser> UpdateEmail(string currentEmail, string newEmail);
		Task<IUser> SetPassword(string email, string password);
        Task<bool> Delete(string email);
        Task RequestResetPassword(string email);
        Task ResetPassword(string email, string resetPasswordToken);
        Task<bool> ValidatePassword(string email, string password);
    }
}