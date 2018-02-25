using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechDevs.Core.UserManagement
{
    public interface IUserRepository
    {
        Task<List<IUser>> GetAll();
        Task<IUser> Insert(IUser user);
        Task<IUser> SetEmail(IUser user, string email);
        Task<IUser> SetName(IUser user, string firstName, string lastName);
        Task<IUser> FindByEmail(string email);
		Task Delete(IUser user);
        Task<bool> UserExists(string email);
    }

    public class UserRepository : IUserRepository
    {
        private readonly List<IUser> Users;

        public UserRepository()
        {
            Users = new List<IUser>();
            Users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Steve",
                LastName = "Kent",
                EmailAddress = "stevekent55@gmail.com",
                AgreedToTerms = true
            });
            Users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Adam",
                LastName = "Fox",
                EmailAddress = "amobilefox@gmail.com",
                AgreedToTerms = true
            });
        }

        public Task<List<IUser>> GetAll()
        {
            var result = Users.ToList();
            return Task.FromResult(result);
        }

        public Task<IUser> Insert(IUser user)
        {
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<IUser> SetEmail(IUser user, string email)
        {
            var existingUser = Users.FirstOrDefault(x => x.Id == user.Id);
            existingUser.EmailAddress = email;
            return Task.FromResult(existingUser);
        }

        public Task<IUser> SetName(IUser user, string firstName, string lastName)
        {
            var existingUser = Users.FirstOrDefault(x => x.Id == user.Id);
            existingUser.FirstName = firstName;
            existingUser.LastName = lastName;
            return Task.FromResult(existingUser);
        }

        public Task Delete(IUser user)
        {
            var existingUser = Users.FirstOrDefault(x => x.Id == user.Id);
            Users.Remove(existingUser);
            return Task.CompletedTask;
        }

        public Task<IUser> FindByEmail(string email)
        {
            var user = Users.FirstOrDefault(x => x.EmailAddress == email);
            return Task.FromResult(user);
        }

        public Task<bool> UserExists(string email)
        {
            var result = Users.Any(x => x.EmailAddress == email);
            return Task.FromResult(result);
        }
    }
}