using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration;

namespace OpenWindowsPatchMan.Agent.Core.Database;
    public class PatchManDbContextFactory : IDesignTimeDbContextFactory<PatchManDbContext>
    {
        public PatchManDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PatchManDbContext>();

        // Provide the connection string or configuration here
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString);

            return new PatchManDbContext(optionsBuilder.Options);
        }
    }
