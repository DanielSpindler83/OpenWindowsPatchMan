using Microsoft.Extensions.Logging;
using OpenWindowsPatchMan.Agent.Core;
using OpenWindowsPatchMan.Agent.Core.Models;
using OpenWindowsPatchMan.Agent.Core.Models.Enums;
using WUApiLib;

namespace OpenWindowsPatchMan.Agent.Core.Services;

public class PatchManUpdateChecker : IPatchManUpdateChecker
{
    private readonly ILogger<PatchManUpdateChecker> _logger;

    public PatchManUpdateChecker(ILogger<PatchManUpdateChecker> logger)
    {
        _logger = logger;
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
                    UpdateCheckTime = updateCheckTime,
                    Title = update.Title,
                    Description = update.Description,
                    DownloadSizeMB = downloadSizeMB,
                    DatePublished = update.LastDeploymentChangeTime,
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

        return updatesInfo;
    }
}