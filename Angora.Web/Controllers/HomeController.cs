using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Angora.Services;

namespace Angora.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            ViewBag.Foo = ServiceManager.GetService<IFooService>().DoSomething(HttpContext.Request.UserAgent);
            return View();
        }
	}
}