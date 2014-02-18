using System;
using System.Collections.Generic;
using System.Data.Entity;
using Angora.Data.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Angora.Data
{
    [DbConfigurationType(typeof(AngoraConfiguration))]
    public class AngoraContext : IdentityDbContext<AngoraUser>
    {
        public DbSet<Foo> Foos { get; set; }

    }
}
