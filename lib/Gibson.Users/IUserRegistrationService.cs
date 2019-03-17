using System;
using System.Threading.Tasks;
using Gibson.Common.Models;

namespace Gibson.Users
{
    public interface IUserRegistrationService
    {
        Task<User> RegisterUser(UserRegistration reg, Guid clientId);
    }
}