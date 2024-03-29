﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;

namespace Angora.Web
{
    public class AngoraWeb : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
