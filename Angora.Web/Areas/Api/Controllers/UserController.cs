using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Angora.Services;
using Microsoft.AspNet.Identity;

namespace Angora.Web.Areas.Api.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private IAngoraUserService _userService;
        public UserController(IAngoraUserService userService)
        {
            _userService = userService;
        }

        // GET api/<controller>/login
        [HttpGet]
        [Route("login")]
        public string Get(string provider, string userId)
        {
            var user = _userService.FindUser(new UserLoginInfo(provider, userId));
            return user.Id;
        }

    }
}
