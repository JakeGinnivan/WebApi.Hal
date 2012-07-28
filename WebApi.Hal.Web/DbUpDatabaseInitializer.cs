using System.Data.Entity;
using DbUp;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web
{
    public class DbUpDatabaseInitializer : IDatabaseInitializer<BeerDbContext>
    {
        readonly string connectionString;

        public DbUpDatabaseInitializer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void InitializeDatabase(BeerDbContext context)
        {
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(typeof(WebApiApplication).Assembly)
                .Build()
                .PerformUpgrade();
        }
    }
}