using System;
using System.Data.Entity;

namespace Angora.Data.Repository
{
    class AngoraDbContext : DbContext
    {
        public AngoraDbContext()
            : base("AngoraDbContext")
        {
            throw new NotImplementedException();
        }
    }
}
