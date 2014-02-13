using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Angora.Data
{
    [DbConfigurationType(typeof(AngoraConfiguration))]
    public class AngoraContext : DbContext
    {
        public DbSet<Foo> Foos { get; set; }

    }
}
