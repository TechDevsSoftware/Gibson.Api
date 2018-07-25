using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Route("api/MyVehicles")]
    public class MyVehiclesController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetMyVehicles()
        {
            return await Task.FromResult(new OkResult());
        }

    }
}