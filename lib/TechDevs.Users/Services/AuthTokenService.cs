using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        public string CreateToken(string userId, string clientKey)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim("Gibson-ClientKey", clientKey)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = tokenHandler.WriteToken(token);
            return result;
        }

        public string UserIdFromToken(string token, string clientIdorKey)
        {
            if (token == null) throw new Exception("Token missing. Cannot authenticate user");
            if (clientIdorKey == null) throw new Exception("ClientIdOrKey missing. Cannot autenticate user");

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) throw new Exception("Jwt token cannot be read");
            var jwt = handler.ReadJwtToken(token);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            return userId;
        }
    }
}