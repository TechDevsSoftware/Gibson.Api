using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechDevs.Accounts.WebService
{
    public static class ExtMethods
    {
        public static string UserId(this Controller controller)
        {
            var user = controller.User;
            var val = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            return val;
        }
    }
}
