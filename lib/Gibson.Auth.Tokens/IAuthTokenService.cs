using System;
using Gibson.Common.Enums;

namespace Gibson.Auth.Tokens
{

    public interface IAuthTokenService
    {
        string CreateToken(Guid userId, string clientKey, Guid clientId, GibsonUserType userType);
        bool ValidateToken(string token);
    }
}
