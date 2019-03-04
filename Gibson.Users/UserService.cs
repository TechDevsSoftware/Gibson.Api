using System;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.Users
{
    public class UserService
    {
        private readonly IUserRepository repo;

        public UserService(IUserRepository repo)
        {
            this.repo = repo;
        }

        public async Task<User> FindByUsername(string username, GibsonUserType userType, Guid clientId)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (clientId == Guid.Empty) throw new ArgumentNullException(nameof(clientId));
            var result = await repo.GetUserByUserName(username, userType, clientId);
            return result;
        }

        public async Task<User> FindById(Guid id, Guid clientId)
        {
            if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));
            if (clientId == Guid.Empty) throw new Exception(nameof(clientId));
            var result = await repo.FindById(id, clientId);
            return result;
        }

        public async Task<User> UpdateUserProfile(Guid id, UserProfile userProfile, Guid clientId)
        {
            if(userProfile == null) throw new Exception("User Profile is null");
            var user = await repo.FindById(id, clientId);
            if (user == null) throw new Exception("User not found");
            user.UserProfile = userProfile;
            var result = await repo.Update(user, clientId);
            return result;
        }

        public async Task DeleteUser(Guid id, Guid clientId)
        {
            if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));
            if (clientId == Guid.Empty) throw new ArgumentNullException(nameof(clientId));
            await repo.Delete(id, clientId);
        }
    }
}
