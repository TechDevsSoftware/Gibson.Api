using Microsoft.Extensions.Options;
using TechDevs.Shared.Models;
using TechDevs.Shared.Utils;

namespace TechDevs.Accounts.Repositories
{
    public abstract class UserRepository : AuthUserBaseRepository<AuthUser>
    {
        public UserRepository(IOptions<MongoDbSettings> dbSettings, IStringNormaliser normaliser) : base(dbSettings, normaliser)
        {
        }
    }
}
