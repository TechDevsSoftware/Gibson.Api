﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TechDevs.Accounts
{
    public class AuthTokenService<TAuthUser> : IAuthTokenService<TAuthUser> where TAuthUser : AuthUser, new()
    {
        private readonly IAuthUserService<TAuthUser> _accountService;
        private readonly string _tokenSecret;

        public AuthTokenService(IAuthUserService<TAuthUser> accountService)
        {
            _accountService = accountService;
            _tokenSecret = "TechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKeyTechDevsKey";
        }

        public string CreateToken(string userId, string requestedClaims, string clientId)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var result = tokenHandler.WriteToken(token);
            return result;
        }

        // private async Task<JwtBuilder> BuildPayload(JwtBuilder builder, string userId, string requestedClaims, string clientId)
        // {
        //     var user = await _accountService.GetById(userId, clientId);
        //     if (user == null) throw new Exception("User not found");
        //     return await BuildPayload(builder, user, requestedClaims);
        // }

        // private Task<JwtBuilder> BuildPayload(JwtBuilder builder, TAuthUser user, string requestedClaims)
        // {
        //     if (requestedClaims.Contains("profile"))
        //     {
        //         builder = builder
        //             .AddClaim(ClaimName.GivenName, user.FirstName)
        //             .AddClaim(ClaimName.FamilyName, user.LastName)
        //             .AddClaim(ClaimName.FullName, $"{user.FirstName} {user.LastName}")
        //             .AddClaim(ClaimName.PreferredEmail, user.EmailAddress);
        //     }
        //     return Task.FromResult(builder);
        // }
    }
}