using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly IAccountService _accountService;
        private readonly string _tokenSecret;

        public AuthTokenService(IAccountService accountService)
        {
            _accountService = accountService;
            _tokenSecret = "techdevs";
        }

        public async Task<string> CreateToken(string userId, string requestedClaims)
        {
            var builder = new JwtBuilder()
                  .WithAlgorithm(new HMACSHA256Algorithm())
                  .WithSecret(_tokenSecret);

            builder = await BuildPayload(builder, userId, requestedClaims);
            var token = builder.Build();
            return token;
        }

        private async Task<JwtBuilder> BuildPayload(JwtBuilder builder, string userId, string requestedClaims)
        {
            var user = await _accountService.GetById(userId);
            if (user == null) throw new Exception("User not found");
            return await BuildPayload(builder, user, requestedClaims);
        }

        private Task<JwtBuilder> BuildPayload(JwtBuilder builder, IUser user, string requestedClaims)
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