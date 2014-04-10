using System;
using System.Data.Entity;
using Angora.Data;
using Angora.Data.Models;
using Angora.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;

namespace Angora.Web.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // Services
            container.RegisterType<IEventService, EventService>();
            container.RegisterType<IFooCDNService, FooCDNService>();
            container.RegisterType<IAngoraUserService, AngoraUserService>();

            // DB stuff
            container.RegisterType<DbConfiguration, AngoraDbConfiguration>();
            container.RegisterType<GenericRepository<BaseModel>>();
            container.RegisterType<IUnitOfWork, UnitOfWork>(new PerRequestLifetimeManager());
            container.RegisterType<DbContext, AngoraDbContext>(new PerRequestLifetimeManager());

            // User Stuff
            container.RegisterType<IUserStore<AngoraUser>, UserStore<AngoraUser>>();
            container.RegisterType<UserManager<AngoraUser>>();

            //var userManager = new UserManager<AngoraUser>(container.<UserStore<AngoraUser>>());
            //userManager.UserValidator = new UserValidator<AngoraUser>(userManager) { AllowOnlyAlphanumericUserNames = false };
            //container.RegisterInstance<UserManager<AngoraUser>>(userManager);
        }
    }
}