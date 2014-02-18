using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Angora.Data;
using System.Data.Entity;
using System.Web.Optimization;

namespace Angora.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // TODO tweak once we're hooked up to Azure
            Database.SetInitializer<Data.AngoraContext>(new DropCreateDatabaseAlways<AngoraContext>());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
