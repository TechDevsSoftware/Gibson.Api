using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TechDevs.Accounts.WebService.Controllers
{
    public class OpenIdConfiguration
    {
        public string issuer { get; set; }
        public string jwks_uri { get; set; }
        public string authorization_endpoint { get; set; }
        public string token_endpoint { get; set; }
        public string userinfo_endpoint { get; set; }
        public string end_session_endpoint { get; set; }
        public string check_session_iframe { get; set; }
        public string revocation_endpoint { get; set; }
        public string introspection_endpoint { get; set; }
        public bool frontchannel_logout_supported { get; set; }
        public bool frontchannel_logout_session_supported { get; set; }
        public bool backchannel_logout_supported { get; set; }
        public bool backchannel_logout_session_supported { get; set; }
        public List<string> scopes_supported { get; set; }
        public List<string> claims_supported { get; set; }
        public List<string> grant_types_supported { get; set; }
        public List<string> response_types_supported { get; set; }
        public List<string> response_modes_supported { get; set; }
        public List<string> token_endpoint_auth_methods_supported { get; set; }
        public List<string> subject_types_supported { get; set; }
        public List<string> id_token_signing_alg_values_supported { get; set; }
        public List<string> code_challenge_methods_supported { get; set; }
    }


    [Produces("application/json")]
    [Route(".well-known/openid-configuration")]
    public class OpenIdController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var config = new OpenIdConfiguration
            {
                issuer = "http://localhost:5101"
            };

            return new OkObjectResult(config);
        }

    }
}