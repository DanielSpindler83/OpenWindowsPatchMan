using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenWindowsPatchMan.Agent.Core;
using OpenWindowsPatchMan.Agent.Core.Models;

namespace OpenWindowsPatchMan.Agent.Service;

public class PatchManWorkerService : BackgroundService
{
    private readonly ILogger<PatchManWorkerService> _logger;
    private readonly IPatchManUpdateService _updateService;
    private readonly IPatchManDatabaseService _databaseService;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _checkInterval;

    public PatchManWorkerService(
        ILogger<PatchManWorkerService> logger,
        IConfiguration configuration,
        IPatchManUpdateService updateService,
        IPatchManDatabaseService databaseService)
    {
        _logger = logger;
        _configuration = configuration;
        _updateService = updateService;
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
            List<WindowsUpdateInfo> updatesInfo = _updateService.CheckForUpdates();

            // Filter updates based on criteria
            List<WindowsUpdateInfo> filteredUpdatesInfo = _updateService.FilterUpdates(updatesInfo);

            // Save update info to database
            _databaseService.SaveUpdateInfo(filteredUpdatesInfo);

            // Install updates
            //_updateService.InstallUpdates(filteredUpdatesInfo);

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}