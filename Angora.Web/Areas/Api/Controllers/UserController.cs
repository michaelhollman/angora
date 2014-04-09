using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Angora.Web.Areas.Api.Controllers
{
    public class UserController : ApiController
    {
        // GET api/<controller>/login
        [HttpGet]
        [Route("login")]
        public string Get(string provider, string id, string accessToken)
        {

            return "value";
        }

    }
}
