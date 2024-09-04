using Microsoft.Extensions.Logging;
using OpenWindowsPatchMan.Agent.Core;
using OpenWindowsPatchMan.Agent.Core.Models;
using OpenWindowsPatchMan.Agent.Core.Models.Enums;
using WUApiLib;

namespace OpenWindowsPatchMan.Agent.Core.Services;

public class PatchManUpdateService : IPatchManUpdateService
{
    private readonly ILogger<PatchManUpdateService> _logger;
    private readonly IPatchManDatabaseService _databaseService;

    public PatchManUpdateService(ILogger<PatchManUpdateService> logger, IPatchManDatabaseService databaseService)
    {
        _logger = logger;
        _databaseService = databaseService;
    }

    public List<WindowsUpdateInfo> CheckForUpdates()
    {
        List<WindowsUpdateInfo> updatesInfo = new List<WindowsUpdateInfo>();

        try
        {
            UpdateSession updateSession = new UpdateSession();
            IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();
            ISearchResult searchResult = updateSearcher.Search("DeploymentAction=*");

            var updateCheckTime = DateTimeOffset.Now;

            foreach (IUpdate update in searchResult.Updates)
            {
                double downloadSizeMB = Math.Round((double)update.MaxDownloadSize / 1024.0 / 1024.0, 2);

                WindowsUpdateInfo updateInfo = new WindowsUpdateInfo
                {
                    UpdateId = update.Identity.UpdateID,
                    UpdateCheckTime = updateCheckTime,
                    Title = update.Title,
                    Description = update.Description,
                    DownloadSizeMB = downloadSizeMB,
                    DatePublished = update.LastDeploymentChangeTime,
                    RevisionNumber = update.Identity.RevisionNumber,
                    DeploymentAction = ((DeploymentActionEnum)update.DeploymentAction).ToString(),
                    IsBeta = update.IsBeta,
                    IsDownloaded = update.IsDownloaded,
                    IsHidden = update.IsHidden,
                    IsInstalled = update.IsInstalled,
                    IsMandatory = update.IsMandatory,
                    InstallationRebootBehavior = ((InstallationRebootBehaviorEnum)update.InstallationBehavior.RebootBehavior).ToString(),
                    IsUninstallable = update.IsUninstallable,
                    ReleaseNotes = update.ReleaseNotes ?? String.Empty,
                    UninstallationNotes = update.UninstallationNotes,
                    UninstallationRebootBehavior = update.UninstallationBehavior?.RebootBehavior != null
                        ? ((InstallationRebootBehaviorEnum)update.UninstallationBehavior.RebootBehavior).ToString()
                        : string.Empty,
                    Type = update.Type.ToString(),
                    SupportUrl = update.SupportUrl,
                };


                foreach (string kbId in update.KBArticleIDs)
                {
                    updateInfo.KBArticleIDs.Add(kbId);
                }

                foreach (ICategory category in update.Categories)
                {
                    updateInfo.Categories.Add(category.Name);
                }

                foreach (string url in update.MoreInfoUrls)
                {
                    updateInfo.MoreInfoUrls.Add(url);
                }

                foreach (string supersededUpdateId in update.SupersededUpdateIDs)
                {
                    updateInfo.SupersededUpdateIDs.Add(supersededUpdateId);
                }

                foreach (string securityBulletinId in update.SecurityBulletinIDs)
                {
                    updateInfo.SecurityBulletinIDs.Add(securityBulletinId);
                }

                foreach (string uninstallationStep in update.UninstallationSteps)
                {
                    updateInfo.UninstallationSteps.Add(uninstallationStep);
                }

                foreach (IUpdate bundledUpdate in update.BundledUpdates)
                {
                    foreach (string kbId in bundledUpdate.KBArticleIDs)
                    {
                        updateInfo.BundledUpdates.Add(kbId);
                    }
                }

                updatesInfo.Add(updateInfo);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking for updates");
        }

        _databaseService.SaveUpdateInfo(updatesInfo);

        return updatesInfo;
    }

    public List<WindowsUpdateInfo> FilterUpdates(List<WindowsUpdateInfo> updates)
    {
        // Example filtering logic
        return updates.Take(1).ToList(); // for testing purposes, only return the first update
    }

    public void InstallUpdates(List<WindowsUpdateInfo> updates)
    {
        UpdateSession updateSession = new UpdateSession();
        IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();
        IUpdateDownloader updateDownloader = updateSession.CreateUpdateDownloader();

        foreach (var updateInfo in updates)
        {
            IUpdate update = FetchUpdate(updateInfo);
            if (update != null)
            {
                updateDownloader.Updates = (UpdateCollection)update;
                updateDownloader.Download();

                updateInstaller.Updates = (UpdateCollection)update;
                _logger.LogInformation($"Starting installation for update: {update.Title}");
                IInstallationResult installationResult = updateInstaller.Install();

                _logger.LogInformation($"Installation result for update {update.Title}: {installationResult.ResultCode}");
                _logger.LogInformation($"Installation result for update {update.Title}: {installationResult.HResult}");
                _logger.LogInformation($"Reboot required for update {update.Title}: {installationResult.RebootRequired}");
            }
        }
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

    public void RetrieveUpdateHistory()
    {
        // https://stackoverflow.com/questions/815340/how-do-i-get-a-list-of-installed-updates-and-hotfixes

        var updateSession = new UpdateSession();
        var updateSearcher = updateSession.CreateUpdateSearcher();
        var count = updateSearcher.GetTotalHistoryCount();
        var history = updateSearcher.QueryHistory(0, count);

        for (int i = 0; i < count; ++i)
            Console.WriteLine(history[i].Title);
    }
}