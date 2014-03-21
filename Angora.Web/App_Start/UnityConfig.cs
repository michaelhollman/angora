using System.Data.Entity;
using System.Web.Mvc;
using Angora.Data;
using Angora.Data.Models;
using Angora.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Infrastructure;
using Microsoft.Practices.Unity;
using Unity.Mvc5;

namespace Angora.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            // TODO: determine how much of this should be per-request/session/something?

            // Services
            container.RegisterType<IEventService, EventService>();
            container.RegisterType<IFooCDNService, FooCDNService>();
            container.RegisterType<IAngoraUserService, AngoraUserService>();

            // DB stuff
            container.RegisterType<DbContext, AngoraDbContext>();
            container.RegisterType<IObjectContextAdapter, AngoraDbContext>();
            container.RegisterType<DbConfiguration, AngoraDbConfiguration>();
            container.RegisterType<GenericRepository<BaseModel>>();

            // TODO: after we get our unit of work in place, we need to add that here

            // User Stuff
            var userManager = new UserManager<AngoraUser>(new UserStore<AngoraUser>(container.Resolve<DbContext>()));
            userManager.UserValidator = new UserValidator<AngoraUser>(userManager) { AllowOnlyAlphanumericUserNames = false };
            container.RegisterInstance<UserManager<AngoraUser>>(userManager);

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}