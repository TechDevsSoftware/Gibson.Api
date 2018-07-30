using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public interface IMyVehicleService
    {
        Task<IUser> AddVehicle(UserVehicle vehicle, string userId);
        Task<IUser> RemoveVehicle(string registration, string userId);
    }

}