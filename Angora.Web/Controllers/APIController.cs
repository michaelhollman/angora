using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Angora.Data;


namespace Angora.Web.Controllers
{
    public class APIController : ApiController
    {
        [AllowAnonymous]
        [Route("login")]
        public IHttpActionResult AttemptLogin(string loginId, string provider)
        {
            
        }
    }
}
