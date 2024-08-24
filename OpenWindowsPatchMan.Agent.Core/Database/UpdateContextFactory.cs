using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration;

namespace OpenWindowsPatchMan.Agent.Core.Database;
    public class UpdateContextFactory : IDesignTimeDbContextFactory<UpdateContext>
    {
        public UpdateContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UpdateContext>();

        // Provide the connection string or configuration here
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString);

            return new UpdateContext(optionsBuilder.Options);
        }
    }
