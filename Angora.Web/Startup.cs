using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Angora.Web.Startup))]
namespace Angora.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}