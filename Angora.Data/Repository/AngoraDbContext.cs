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
    }
}
