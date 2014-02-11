using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Angora.Web.Areas.Api.Controllers
{
    [RouteArea("api")]
    [RoutePrefix("foo")]
    public class FooController : ApiBaseController
    {
        [Route]
        public ActionResult GetFoo()
        {
            return Json("bar", JsonRequestBehavior.AllowGet);
        }

        [Route("{p}")]
        public ActionResult FooFoo(string p)
        {
            return Json("lol " + p, JsonRequestBehavior.AllowGet);
        }

    }
}
