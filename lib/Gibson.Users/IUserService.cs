using System;
using System.Threading.Tasks;
using Gibson.Common.Enums;
using Gibson.Common.Models;

namespace Gibson.Users
{
    public interface IUserService
    {
        Task<User> FindByUsername(string username, GibsonUserType userType, Guid clientId);
        Task<User> FindByProviderId(string providerId, GibsonUserType userType, Guid clientId);
        Task<User> FindById(Guid id, Guid clientId);
        Task<User> UpdateUserProfile(Guid id, UserProfile userProfile, Guid clientId);
        Task DeleteUser(Guid id, Guid clientId);
    }
}