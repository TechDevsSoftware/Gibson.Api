using TechDevs.Shared.Models;
using TechDevs.Users;

namespace TechDevs.Employees
{
    public interface IEmployeeService : IUserService<Employee> { }

}