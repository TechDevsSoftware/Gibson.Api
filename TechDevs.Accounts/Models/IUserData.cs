using System.Collections.Generic;

namespace TechDevs.Accounts
{
    public interface IUserData
    {
        List<UserVehicle> MyVehicles { get; set; }
        List<VehicleListing> SavedVehicles { get; set; }
        List<ServiceHistory> ServiceHistories { get; set; }
    }
}