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
            //SetDefaultConnectionFactory(new SqlAzureExecutionStrategy());
            SetDatabaseInitializer(new DropCreateDatabaseIfModelChanges<AngoraDbContext>());
        }
    }
}
