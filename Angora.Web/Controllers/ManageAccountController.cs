using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Angora.Services;
using Angora.Web.Models;

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
        [HttpGet]
        [Authorize]
        [Route("account")]
        public ActionResult Index(ManageAccountViewModel param = null)
        {
            var model = param ?? new ManageAccountViewModel();

            model.User = _userService.FindUserById(User.Identity.GetUserId());
            return View("Index", model);
        }


        [HttpPost]
        [Authorize]
        [Route("account/update")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserInfo(ManageAccountViewModel param)
        {
            var model = new ManageAccountViewModel();

            var user = _userService.FindUserById(User.Identity.GetUserId());
            user.FirstName = param.User.FirstName ?? user.FirstName;
            user.LastName = param.User.LastName ?? user.LastName;
            user.EmailAddress = param.User.EmailAddress ?? user.EmailAddress;
            user.Location = param.User.Location ?? user.Location;
            user.Birthday = param.User.Birthday != null ? param.User.Birthday : user.Birthday;
            user.UserName = String.Format("{0} {1}", user.FirstName, user.LastName);

            if (_userService.UpdateUser(user))
            {
                model.Successes.Add("Successfully updated account");
                model.Infos.Add("info!!");
                model.Errors.Add("error =[");
            }
            else
            {
                model.Errors.Add("There was an error trying to update your account");
            }

            return Index(model);
        }
    }
}