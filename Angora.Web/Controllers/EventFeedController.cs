using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Angora.Services;
using Angora.Web.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;

namespace Angora.Web.Controllers
{
    public class EventFeedController : Controller
    {
        //
        // GET: /EventFeed/
        [Route("Events")]
        public ActionResult Index()
        {
            IEventService service = ServiceManager.GetService<IEventService>();
            EventFeedViewModel model = new EventFeedViewModel();
            model.Events = service.FindEventsByUserId(User.Identity.GetUserId());
            return View(model);
        }
	}
}