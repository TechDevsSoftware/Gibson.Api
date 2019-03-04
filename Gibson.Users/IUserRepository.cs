using System;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.Users
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUserName(string username, GibsonUserType userType, Guid clientId);
        Task<User> GetUserByProviderId(string providerId, GibsonUserType userType, Guid clientId);
    }
}