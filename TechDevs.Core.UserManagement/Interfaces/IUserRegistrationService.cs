using System.Threading.Tasks;

namespace TechDevs.Core.UserManagement.Interfaces
{
    public interface IUserRegistrationService
    {
        Task<IUser> RegisterUser(IUserRegistration userRegistration);
        Task ValidateCanRegister(IUserRegistration userRegistration);
    }
}