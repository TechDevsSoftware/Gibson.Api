using System.Threading.Tasks;

namespace TechDevs.Core.UserManagement.Interfaces
{
    public interface IUserRepository
    {
        Task<IUser> GetUser(string userId);
        Task<IUser> CreateUser(IUserRegistration registration);
        Task<bool> EmailAlreadyRegistered(string email);
    }
}