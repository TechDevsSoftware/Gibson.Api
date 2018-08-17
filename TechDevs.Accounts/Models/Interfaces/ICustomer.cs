using System.Collections.Generic;

namespace TechDevs.Accounts
{
    public interface ICustomer: IAuthUser
    {        
        ICustomerData CustomerData { get; set; }
    }

    public interface ICustomerData
    {
        List<CustomerVehicle> MyVehicles { get; set; }
        List<VehicleListing> SavedVehicles { get; set; }
        List<ServiceHistory> ServiceHistories { get; set; }
    }
}