using System.Collections.Generic;

namespace TechDevs.Accounts
{
    public interface IClient
    {
        string BusinessTitle { get; set; }
        ClientTheme Theme { get; set; }
        List<ICustomer> Customers { get; set; }
        List<IEmployee> Employees { get; set; }
        string Id { get; set; }
        string Name { get; set; }
    }
}