using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechDevs.Accounts.ExtMethods
{
    public static class ExtMethods
    {

        public static string UserId(this Controller controller)
        {
                return controller.User.FindFirst("sub")?.Value;
        }
    }
}
