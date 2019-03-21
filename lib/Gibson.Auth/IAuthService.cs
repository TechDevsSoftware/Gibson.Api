using System;
using System.Threading.Tasks;
using Gibson.Common.Enums;
using Gibson.Common.Models;

namespace Gibson.Auth
{
    public interface IAuthService
    {
        Task<string> Login(LoginRequest req, GibsonUserType userType, Guid clientId);
        bool ValidateToken(string token);
    }
}