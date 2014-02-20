using System;
using System.Configuration;
using System.Data.Entity;
using Angora.Data.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Angora.Data
{
    public class AngoraDbContext : IdentityDbContext<AngoraUser>
    {
        //TODO eventually get conditional connection strings for debug/release
        public AngoraDbContext()
            : base(ConfigurationManager.ConnectionStrings["AngoraDbConnection"].ConnectionString)
        {
        }

        public DbSet<Foo> Foos { get; set; }

    }
}
