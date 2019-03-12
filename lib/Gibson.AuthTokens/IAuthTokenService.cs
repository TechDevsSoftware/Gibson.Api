using System;

namespace Gibson.AuthTokens
{

    public interface IAuthTokenService
    {
        string CreateToken(Guid userId, string clientKey, Guid clientId);
        bool ValidateToken(string token);
    }
}
