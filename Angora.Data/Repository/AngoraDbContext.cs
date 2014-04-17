using System.Configuration;
using System.Data.Entity;
using Angora.Data.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Angora.Data
{
    public class AngoraDbContext : IdentityDbContext<AngoraUser>
    {
        public AngoraDbContext() : base(ConfigurationManager.ConnectionStrings["AngoraDbConnection"].ConnectionString, false) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventScheduler> EventSchedulers { get; set; }
        public DbSet<EventSchedulerResponse> EventSchedulerResponses { get; set; }
        public DbSet<EventTime> EventTimes { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
