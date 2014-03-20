using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Angora.Services;

namespace Angora.Web.Controllers
{
    public class ManageAccountController : Controller
    {

        private IAngoraUserService _userService;

        public ManageAccountController()
            : this(ServiceManager.GetService<IAngoraUserService>())
        {
        }

        public ManageAccountController(IAngoraUserService userService)
        {
            _userService = userService;
        }

        //
        // GET: /ManageAccount/
        [Authorize]
        public ActionResult Index()
        {






            return View();
        }
    }
}