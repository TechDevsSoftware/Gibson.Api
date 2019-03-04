using System;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace Gibson.Users
{
    public interface IUserRegistrationService
    {
        Task<User> RegisterUser(UserRegistration reg, Guid clientId);
    }
}