using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenWindowsPatchMan.Agent.Core;
using OpenWindowsPatchMan.Agent.Core.Database;
using OpenWindowsPatchMan.Agent.Core.Services;


namespace OpenWindowsPatchMan.Agent.Service;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<PatchManWorkerService>();

                // Register IDbContextFactory for UpdateContext
                services.AddDbContextFactory<UpdateContext>(options =>
                    options.UseSqlite(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                // Register services
                services.AddSingleton<IPatchManUpdateChecker, PatchManUpdateChecker>();
                services.AddSingleton<IPatchManUpdateFilter, PatchManUpdateFilter>();
                services.AddSingleton<IPatchManUpdateInstaller, PatchManUpdateInstaller>();
                services.AddSingleton<IPatchManDatabaseService, PatchManDatabaseService>();
            });
}