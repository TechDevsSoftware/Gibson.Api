using System.Threading.Tasks;

namespace TechDevs.Core.UserManagement.Interfaces
{
    public interface IUserProfileService
    {
        Task<IUser> GetUserProfile(string email);
    }
}