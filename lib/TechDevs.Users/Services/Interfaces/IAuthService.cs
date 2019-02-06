using System;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Users
{
    public interface IAuthService<T> where T : AuthUser, new()
    {
        Task<string> Login(string email, string password, Guid clientId, string clientKey);
        Task<bool> ValidatePassword(string email, string password, string clientId);
        bool ValidateToken(string token);
    }

}