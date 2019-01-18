using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;

namespace TechDevs.Users
{
    public abstract class UserRepository : AuthUserBaseRepository<AuthUser>
    {
        public UserRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser) : base(dbSettings, normaliser)
        {
        }
    }
}
