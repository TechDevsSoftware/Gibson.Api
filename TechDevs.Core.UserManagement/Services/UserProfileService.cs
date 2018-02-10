using System;
using System.Threading.Tasks;
using TechDevs.Core.UserManagement.Interfaces;

namespace TechDevs.Core.UserManagement.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _repo;

        public UserProfileService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<IUser> GetUserProfile(string userId)
        {
            var user = await _repo.GetUser(userId);
            if (user == null) throw new Exception("User could not be found");
            return user;
        }
    }
}
