using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public class AuthTokenService<TAuthUser> : IAuthTokenService<TAuthUser> where TAuthUser : AuthUser, new()
    {
        private readonly IAuthUserService<TAuthUser> _accountService;
        private readonly string _tokenSecret;

        public AuthTokenService(IAuthUserService<TAuthUser> accountService)
        {
            _accountService = accountService;
            _tokenSecret = "techdevstechdevstechdevstechdevstechdevstechdevstechdevstechdevstechdevstechdevs";
        }

        public async Task<string> CreateToken(string userId, string requestedClaims, string clientId)
        {
            var builder = new JwtBuilder()
                  .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
                  .AddClaim("sub", userId)
                  .WithAlgorithm(new HMACSHA256Algorithm())
                  .WithSecret(_tokenSecret);

            builder = await BuildPayload(builder, userId, requestedClaims, clientId);
            var token = builder.Build();
            return token;
        }

        private async Task<JwtBuilder> BuildPayload(JwtBuilder builder, string userId, string requestedClaims, string clientId)
        {
            var user = await _accountService.GetById(userId, clientId);
            if (user == null) throw new Exception("User not found");
            return await BuildPayload(builder, user, requestedClaims);
        }

        private Task<JwtBuilder> BuildPayload(JwtBuilder builder, TAuthUser user, string requestedClaims)
        {
            if (requestedClaims.Contains("profile"))
            {
                builder = builder
                    .AddClaim(ClaimName.GivenName, user.FirstName)
                    .AddClaim(ClaimName.FamilyName, user.LastName)
                    .AddClaim(ClaimName.FullName, $"{user.FirstName} {user.LastName}")
                    .AddClaim(ClaimName.PreferredEmail, user.EmailAddress);
            }
            return Task.FromResult(builder);
        }
    }
}