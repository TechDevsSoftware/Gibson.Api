using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechDevs.Core.UserManagement.Interfaces;
using TechDevs.Core.UserManagement.Models;

namespace TechDevs.Core.UserManagement.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly List<IUser> Users;

        public InMemoryUserRepository()
        {
            Users = new List<IUser>();
            Users.Add(new User
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Steve",
                LastName = "Kent",
                EmailAddress = "stevekent55@gmail.com",
                AgreedToTerms = true
            });
            Users.Add(new User
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = "Adam",
                LastName = "Fox",
                EmailAddress = "amobilefox@gmail.com",
                AgreedToTerms = true
            });
        }

        public Task<IUser> CreateUser(IUserRegistration registration)
        {
            IUser newUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                EmailAddress = registration.EmailAddress,
                AgreedToTerms = registration.AggreedToTerms
            };
            Users.Add(newUser);
            return Task.FromResult(newUser);
        }

        public Task<IUser> GetUser(string userId)
        {
            var user = Users.FirstOrDefault(x => x.UserId == userId);
            return Task.FromResult(user);
        }

        public Task<bool> EmailAlreadyRegistered(string email)
        {
            var result = Users.Any(x => x.EmailAddress == email);
            return Task.FromResult(result);
        }
    }
}