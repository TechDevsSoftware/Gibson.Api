using System;

namespace Gibson.Auth.Tokens
{

    public interface IAuthTokenService
    {
        string CreateToken(Guid userId, string clientKey, Guid clientId);
        bool ValidateToken(string token);
    }
}
