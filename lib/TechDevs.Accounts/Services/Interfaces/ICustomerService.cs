﻿using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Accounts
{
    public interface ICustomerService
    {
        Task<Customer> UpdateCustomerData<CustDataType>(string userId, string customerDataPathName, CustDataType custData, string clientId);
    }
}