using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechDevs.Mail;

namespace TechDevs.Accounts.WebService.Controllers
{
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : Controller
    {
        private IEmailer _emailer;

        public TestController(IEmailer emailer)
        {
            _emailer = emailer;
        }

        [HttpGet]
        public IActionResult SendEmail()
        {
            _emailer.SendSecurityEmail("stevekent55@gmail.com", "This is a test", "Bodalkwdnlkadakdlnalkndlkandsk", false);
            return new OkResult();
        }
    }
}