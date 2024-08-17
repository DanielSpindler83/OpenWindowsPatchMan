using Microsoft.Extensions.Logging;
using OpenWindowsPatchMan.Agent.Core.Models;
using WUApiLib;

namespace OpenWindowsPatchMan.Agent.Core.Services;

public class PatchManUpdateInstaller : IPatchManUpdateInstaller
{
    private readonly ILogger<UpdateInstaller> _logger;

    public PatchManUpdateInstaller(ILogger<UpdateInstaller> logger)
    {
        _logger = logger;
    }

    public void InstallUpdates(List<WindowsUpdateInfo> updates)
    {
        UpdateSession updateSession = new UpdateSession();
        IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();
        UpdateCollection updateCollection = new UpdateCollection();
        foreach (var updateInfo in updates)
        {
            // We need to fetch the actual IUpdate object here based on the updateInfo
        }

        updateInstaller.Updates = updateCollection;

        _logger.LogInformation("Starting installation...");
        IInstallationResult installationResult = updateInstaller.Install();

        _logger.LogInformation($"Installation result: {installationResult.ResultCode}");
        _logger.LogInformation($"Reboot required: {installationResult.RebootRequired}");
    }
}