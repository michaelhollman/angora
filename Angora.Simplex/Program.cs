using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Angora.Data.Models;
using Angora.Services;
using Angora.Data;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Angora.Simplex
{
    class Program
    {
        static void Main(string[] args)
        {

            UnityContainer container = new UnityContainer();
            container.RegisterType<ISimplexService, SimplexService>();
            container.RegisterType<IFooCDNService, FooCDNService>();
            container.RegisterType<IEventService, EventService>();
            container.RegisterType<IRepository<Event>, GenericRepository<Event>>();
            container.RegisterType<DbConfiguration, AngoraDbConfiguration>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<DbContext, AngoraDbContext>();
            container.RegisterType<IEventSchedulerService, EventSchedulerService>();
            container.RegisterType<IAngoraUserService, AngoraUserService>();
            container.RegisterType<IPostService, PostService>();
            container.RegisterType<IRSVPService, RSVPService>();
            container.RegisterType<IRepository<EventScheduler>, GenericRepository<EventScheduler>>();
            container.RegisterType<IRepository<EventSchedulerResponse>, GenericRepository<EventSchedulerResponse>>();
            container.RegisterType<IRepository<EventTime>, GenericRepository<EventTime>>();
            container.RegisterType<IRepository<Location>, GenericRepository<Location>>();
            container.RegisterType<IRepository<MediaItem>, GenericRepository<MediaItem>>();
            container.RegisterType<IRepository<Post>, GenericRepository<Post>>();
            container.RegisterType<IRepository<Tag>, GenericRepository<Tag>>();
            container.RegisterType<IRepository<RSVP>, GenericRepository<RSVP>>();

            // User Stuff
            container.RegisterType<IUserStore<AngoraUser>, UserStore<AngoraUser>>();
            container.RegisterType<UserManager<AngoraUser>>();

            var service = container.Resolve<ISimplexService>();
            service.PerformSimplex();
        }
    }
}
