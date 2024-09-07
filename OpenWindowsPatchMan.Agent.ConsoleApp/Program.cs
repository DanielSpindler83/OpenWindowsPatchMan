
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenWindowsPatchMan.Agent.Core;
using OpenWindowsPatchMan.Agent.Core.Database;
using OpenWindowsPatchMan.Agent.Core.Interfaces;
using OpenWindowsPatchMan.Agent.Core.Models;
using OpenWindowsPatchMan.Agent.Core.Services;


namespace OpenWindowsPatchMan.Agent.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load the configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Set up the DI container
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IPatchManUpdateService, PatchManUpdateService>()
                .AddSingleton<IPatchManConfigureWindowsUpdate, PatchManConfigureWindowsUpdate>()
                .AddSingleton<IPatchManDatabaseService, PatchManDatabaseService>()
                .AddDbContextFactory<PatchManDbContext>(options =>
                {
                    options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                })
                .BuildServiceProvider();

            // Resolve the services
            var updatesService = serviceProvider.GetService<IPatchManUpdateService>();
            var configureWindowsUpdateService = serviceProvider.GetService<IPatchManConfigureWindowsUpdate>();
            var databaseService = serviceProvider.GetService<IPatchManDatabaseService>();


            databaseService.InitializeDatabase(); // Ensure the database and table are created

            // Process command line arguments
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "fetch-updates":
                        updatesService.CheckForUpdates();
                        break;
                    case "install-updates":
                        List<WindowsUpdateInfo> updatesToInstall = updatesService.CheckForUpdates();
                        databaseService.SaveUpdateInfo(updatesToInstall);
                        var testing = updatesToInstall.FirstOrDefault(update => !update.IsInstalled && update.DeploymentAction != "OptionalInstallation");
                        List<WindowsUpdateInfo> testingList = new List<WindowsUpdateInfo>();
                        if (testing != null)
                        {
                            testingList.Add(testing);
                        }
                        updatesService?.InstallUpdates(testingList);
                        break;
                    case "configure-windows-update":
                        configureWindowsUpdateService.LockWindowsUpdateGUI();
                        break;
                    case "retrieve-update-history":
                        updatesService?.RetrieveUpdateHistory();
                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("No command provided.");
            }
        }
    }
}