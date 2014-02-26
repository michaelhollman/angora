using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Angora.Data;
using Angora.Data.Models;
using Angora.Web.Models;
using Facebook;

namespace Angora.Web.Controllers
{
    public class InfoGetterController : Controller 
    {
        
        public ActionResult Index()
        {
            InfoGetterModel model = new InfoGetterModel();
            model.UserID = User.Identity.Name;
            
            return View(model);
        }
    }
}