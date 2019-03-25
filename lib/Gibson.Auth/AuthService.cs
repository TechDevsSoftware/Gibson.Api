using System;
using System.Threading.Tasks;
using Gibson.Auth.Crypto;
using Gibson.Auth.Tokens;
using Gibson.Users;
using Google.Apis.Auth;
using Gibson.Clients;
using Gibson.Common.Enums;
using Gibson.Common.Models;

namespace Gibson.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthTokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IClientService _clientService;

        public AuthService(IAuthTokenService tokenService, IUserService userService, IPasswordHasher passwordHasher, IClientService clientService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _passwordHasher = passwordHasher;
            _clientService = clientService;
        }

        public bool ValidateToken(string token)
        {
            return _tokenService.ValidateToken(token);
        }
        
        public async Task<string> Login(LoginRequest req, GibsonUserType userType, Guid clientId)
        {
            if (!string.IsNullOrEmpty(req.ProviderIdToken)) 
                return await LoginViaGoogle(req, userType, clientId);
            else
                return await LoginViaGibson(req, userType, clientId);
        }
        
        private async Task<string> LoginViaGibson(LoginRequest req, GibsonUserType userType, Guid clientId)
        {
            var user = await _userService.FindByUsername(req.Email, userType, clientId);
            var client = await _clientService.GetClient(clientId.ToString());
            if(user == null) throw new Exception("User not found");
            var validPassword = _passwordHasher.VerifyHashedPassword(user?.AuthProfile?.PasswordHash, req.Password);
            if(!validPassword) throw new UnauthorizedAccessException();
            var token = _tokenService.CreateToken(user.Id, client.ShortKey, clientId, userType);
            return token;
        }

        private async Task<string> LoginViaGoogle(LoginRequest req, GibsonUserType userType, Guid clientId)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(req.ProviderIdToken);
            var client = await _clientService.GetClient(clientId.ToString());
            var user = await _userService.FindByProviderId(payload.Subject, userType, clientId);
            if (user == null) throw new Exception("User not found");
            var token = _tokenService.CreateToken(user.Id, client.ShortKey, clientId, userType);
            return token;
        }

        private async Task<User> LogAuthEvent_Login()
        {
            return await Task.FromResult(new User());
        }
    }
}