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
            IUpdate update = FetchUpdate(updateInfo);
            updateCollection.Add(update);
        }

        updateInstaller.Updates = updateCollection;

        _logger.LogInformation("Starting installation...");
        IInstallationResult installationResult = updateInstaller.Install();

        _logger.LogInformation($"Installation result: {installationResult.ResultCode}");
        _logger.LogInformation($"Reboot required: {installationResult.RebootRequired}");
    }

    private IUpdate FetchUpdate(WindowsUpdateInfo updateInfo)
    {
        UpdateSession updateSession = new UpdateSession();
        IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();

        // Create a search criteria based on the updateInfo
        ISearchResult searchResult = updateSearcher.Search($"IsInstalled=0 and UpdateID='{updateInfo.UpdateId}'");

        // Check if any updates match the search criteria
        if (searchResult.Updates.Count == 1)
        {
            // Return the first matching update
            return searchResult.Updates[0];
        }
        else
        {
            // Handle the case when no or multiple updates match the search criteria
            // You can throw an exception, log an error, or handle it based on your application's requirements
            // For now, let's return null
            return null;
        }
    }
}
