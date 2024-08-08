

using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenWindowsPatchMan.ConsoleAgent.Enums;
using WUApiLib;

namespace OpenWindowsPatchMan.ConsoleAgent;

public class PatchManWorkerService : BackgroundService
{
    private readonly ILogger<PatchManWorkerService> _logger;
    private readonly IPatchManUpdateChecker _updateChecker;
    private readonly IPatchManUpdateFilter _updateFilter;
    private readonly IPatchManUpdateInstaller _updateInstaller;
    private readonly IPatchManDatabaseService _databaseService;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _checkInterval;

    public PatchManWorkerService(
        ILogger<PatchManWorkerService> logger,
        IConfiguration configuration,
        IPatchManUpdateChecker updateChecker,
        IPatchManUpdateFilter updateFilter,
        IPatchManUpdateInstaller updateInstaller,
        IPatchManDatabaseService databaseService)
    {
        _logger = logger;
        _configuration = configuration;
        _updateChecker = updateChecker;
        _updateFilter = updateFilter;
        _updateInstaller = updateInstaller;
        _databaseService = databaseService;
        _checkInterval = TimeSpan.FromMinutes(_configuration.GetValue<int>("CheckIntervalMinutes", 60));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _databaseService.InitializeDatabase(); // Ensure the database and table are created

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("PatchManWorkerService running at: {time}", DateTimeOffset.Now);

            // Check for updates
            List<WindowsUpdateInfo> updatesInfo = _updateChecker.CheckForUpdates();

            // Filter updates based on criteria
            List<WindowsUpdateInfo> filteredUpdatesInfo = _updateFilter.FilterUpdates(updatesInfo);

            // Save update info to database
            _databaseService.SaveUpdateInfo(filteredUpdatesInfo);

            // Install updates
            //_updateInstaller.InstallUpdates(filteredUpdatesInfo);

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}