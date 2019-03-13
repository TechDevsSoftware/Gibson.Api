using System;
using System.Threading.Tasks;
using Gibson.AuthTokens;
using Gibson.Users;
using Google.Apis.Auth;
using TechDevs.Shared;
using TechDevs.Shared.Models;

namespace Gibson.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthTokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IAuthTokenService tokenService, IUserService userService, IPasswordHasher passwordHasher)
        {
            _tokenService = tokenService;
            _userService = userService;
            _passwordHasher = passwordHasher;
        }

        public bool ValidateToken(string token)
        {
            return _tokenService.ValidateToken(token);
        }
        
        public async Task<string> Login(LoginRequest req)
        {
            ValidateLoginRequest_PreSubmit(req);
            
            // Get the clientId from the clientKey
            switch (req.Provider)
            {
                case "Gibson":
                    return await LoginViaGibson(req);
                case "Google":
                    return await LoginViaGoogle(req);
                default:
                    throw new Exception("Unsupported auth provider");
            }
        }

        private static void ValidateLoginRequest_PreSubmit(LoginRequest req)
        {
            // Work out the provider by splitting the controller to have two endpoints /auth/login & /auth/social
            // Work out the clientId from the referer url and clientService
            // Work out the clientKey from the referer url
            // Work out the user type from the referer url
            
            if(req.UserType == GibsonUserType.NotSet) throw new ArgumentNullException("User type not set");
            if(string.IsNullOrEmpty(req.Provider)) throw new ArgumentNullException("Provider not set");
            if(req.ClientId == Guid.Empty) throw new ArgumentNullException("ClientId not set");
            if(string.IsNullOrEmpty(req.ClientKey)) throw new ArgumentNullException("Client key not set");
        }

        private async Task<string> LoginViaGibson(LoginRequest req)
        {
            var user = await _userService.FindByUsername(req.Email, req.UserType, req.ClientId);
            if(user == null) throw new Exception();
            var validPassword = _passwordHasher.VerifyHashedPassword(user?.AuthProfile?.PasswordHash, req.Password);
            if(!validPassword) throw new UnauthorizedAccessException();
            return _tokenService.CreateToken(user.Id, req.ClientKey, req.ClientId);
        }

        private async Task<string> LoginViaGoogle(LoginRequest req)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(req.ProviderIdToken);
            var user = await _userService.FindByProviderId(payload.Subject, req.UserType, req.ClientId);
            return _tokenService.CreateToken(user.Id, req.ClientKey, req.ClientId);
        }
    }
}