using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TechDevs.Shared.Models;

namespace TechDevs.Users
{
    public class AuthTokenService<TAuthUser> : IAuthTokenService<TAuthUser> where TAuthUser : AuthUser, new()
    {
        private readonly string _tokenSecret;

        public AuthTokenService()
        {
            _tokenSecret = "TechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKey";
        }

        public string CreateToken(string userId, string clientKey, Guid clientId)
        {
            // Authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim("Gibson-ClientKey", clientKey),
                    new Claim("Gibson-ClientId", clientId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = tokenHandler.WriteToken(token);
            return result;
        }
    }
}