using System.Threading.Tasks;
using TechDevs.Shared.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Gibson.CustomerVehicles
{
    //public class MockCustomerVehicleRepo : ICustomerVehicleRepository
    //{
    //    private List<CustomerVehicle> vehicles = new List<CustomerVehicle>();

    //    public MockCustomerVehicleRepo()
    //    {
    //        // Seed data
    //        vehicles.Add(new CustomerVehicle { Id = Guid.NewGuid(), Registration = "LD66OFZ", Make = "KIA", Model = "CEED", Colour = "BLACK" });
    //    }

    //    public Task<CustomerVehicle> Create(CustomerVehicle entity, Guid customerId, Guid clientId)
    //    {
    //        vehicles.Add(entity);
    //        return Task.FromResult(entity);
    //    }

    //    public Task<List<CustomerVehicle>> FindAll(Guid clientId)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<List<CustomerVehicle>> FindAllByCustomer(Guid customerId, Guid clientId)
    //    {
    //        var results = vehicles;
    //        return Task.FromResult(results);
    //    }

    //    public Task<CustomerVehicle> FindById(Guid id, Guid customerId, Guid clientId)
    //    {
    //        var result = vehicles.FirstOrDefault(x => x.Id == id);
    //        return Task.FromResult(result);
    //    }

    //    public Task<CustomerVehicle> Update(CustomerVehicle entity, Guid customerId, Guid clientId)
    //    {
    //        var index = vehicles.FindIndex(x => x.Id == entity.Id);
    //        if (index == -1) throw new Exception("Vehicle cannot be updated as no matching vehicle found");
    //        vehicles[index] = entity;
    //        return Task.FromResult(entity);
    //    }

    //    public Task Delete(Guid id, Guid customerId, Guid clientId)
    //    {
    //        var index = vehicles.FindIndex(x => x.Id == id);
    //        vehicles.RemoveAt(index);
    //        return Task.CompletedTask;
    //    }

    //    public void Reset()
    //    {
    //        vehicles = new List<CustomerVehicle>();
    //    }

    //    public int RowCount()
    //    {
    //        return vehicles.Count;
    //    }
    //}
}