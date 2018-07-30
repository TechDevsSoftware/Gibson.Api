using System;
using System.Linq;
using System.Threading.Tasks;

namespace TechDevs.Accounts
{
    public class MyVehicleService : IMyVehicleService
    {
        private readonly IUserRepository _userRepo;

        public MyVehicleService(IUserRepository userRepository)
        {
            _userRepo = userRepository;
        }

        public async Task<IUser> AddVehicle(UserVehicle vehicle, string userId)
        {
            var user = await _userRepo.FindById(userId);
            if (user == null) throw new Exception("User not found");

            if (user.UserData.MyVehicles.Any(v => v.Registration == vehicle.Registration)) throw new Exception("Vehicle already added. Cannot duplicate vehicle registrations.");
            
            user.UserData.MyVehicles.Add(vehicle);

            var result = await _userRepo.UpdateUser<UserVehicle>("UserData.MyVehicles", user.UserData.MyVehicles, userId);
            return result;
        }

        public async Task<IUser> RemoveVehicle(string registration, string userId)
        {
            var user = await _userRepo.FindById(userId);
            if (user == null) throw new Exception("User not found");

            user.UserData.MyVehicles = user.UserData.MyVehicles.Where(x => x.Registration != registration).ToList();

            var result = await _userRepo.UpdateUser<UserVehicle>("UserData.MyVehicles", user.UserData.MyVehicles, userId);
            return result;
        }
    }

}