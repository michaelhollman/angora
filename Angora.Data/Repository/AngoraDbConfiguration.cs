using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace Angora.Data
{
    public class AngoraDbConfiguration : DbConfiguration
    {
        public AngoraDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("v11.0"));
            SetDatabaseInitializer<AngoraDbContext>(new DropCreateDatabaseIfModelChanges<AngoraDbContext>());
            // TODO remove mdf/ldf from git repo and configure this to "create if not exists"
        }
    }
}
