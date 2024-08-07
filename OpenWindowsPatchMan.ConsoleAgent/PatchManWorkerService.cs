

using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenWindowsPatchMan.ConsoleAgent.Enums;
using WUApiLib;

namespace OpenWindowsPatchMan.ConsoleAgent;

// https://learn.microsoft.com/en-us/windows/win32/api/_wua/
// https://learn.microsoft.com/en-us/windows/win32/wua_sdk/windows-updateInfo-agent--wua--api-reference
// https://learn.microsoft.com/en-us/windows/win32/wua_sdk/windows-updateInfo-agent-object-model

public class PatchManWorkerService : BackgroundService
{
    private readonly ILogger<PatchManWorkerService> _logger;
    private readonly IConfiguration _configuration;
    private TimeSpan _checkInterval;

    public PatchManWorkerService(ILogger<PatchManWorkerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _checkInterval = TimeSpan.FromMinutes(_configuration.GetValue<int>("CheckIntervalMinutes", 60));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeDatabase();  // Ensure the database and table are created

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("PatchManWorkerService running at: {time}", DateTimeOffset.Now);

            // Checks if automatic updates are enabled
            AutomaticUpdates automaticUpdates = new AutomaticUpdates();
            var automaticUpdatesEnabled = automaticUpdates.ServiceEnabled;

            List<WindowsUpdateInfo> updatesInfo = CheckForUpdates();

            using var connection = new SqliteConnection("Data Source=updates.db");
            foreach (var updateInfo in updatesInfo)
            {
                connection.Execute(@"
                    INSERT INTO WindowsUpdateInfo (
                        UpdateCheckTime, 
                        Title, 
                        Description, 
                        KBArticleIDs, 
                        Categories, 
                        DownloadSizeMB, 
                        MoreInfoUrls, 
                        DatePublished,
                        DeploymentAction,
                        IsBeta,
                        IsDownloaded,
                        IsHidden,
                        IsInstalled,
                        IsMandatory,
                        InstallationRebootBehavior,
                        IsUninstallable,
                        ReleaseNotes,
                        UninstallationSteps,
                        UninstallationNotes,
                        UninstallationRebootBehavior,
                        Type,
                        SupportUrl,
                        SupersededUpdateIDs,
                        SecurityBulletinIDs,
                        BundledUpdates
                    ) VALUES (
                        @UpdateCheckTime, 
                        @Title, 
                        @Description, 
                        @KBArticleIDs, 
                        @Categories, 
                        @DownloadSizeMB, 
                        @MoreInfoUrls, 
                        @DatePublished,
                        @DeploymentAction,
                        @IsBeta,
                        @IsDownloaded,
                        @IsHidden,
                        @IsInstalled,
                        @IsMandatory,
                        @InstallationRebootBehavior,
                        @IsUninstallable,
                        @ReleaseNotes,
                        @UninstallationSteps,
                        @UninstallationNotes,
                        @UninstallationRebootBehavior,
                        @Type,
                        @SupportUrl,
                        @SupersededUpdateIDs,
                        @SecurityBulletinIDs,
                        @BundledUpdates
                    )",
                                new
                                {
                                    UpdateCheckTime = updateInfo.UpdateCheckTime.ToString("o"),
                                    Title = updateInfo.Title,
                                    Description = updateInfo.Description,
                                    KBArticleIDs = string.Join(", ", updateInfo.KBArticleIDs),
                                    Categories = string.Join(", ", updateInfo.Categories),
                                    DownloadSizeMB = updateInfo.DownloadSizeMB,
                                    MoreInfoUrls = string.Join(", ", updateInfo.MoreInfoUrls),
                                    DatePublished = updateInfo.DatePublished.ToString("o"),
                                    DeploymentAction = updateInfo.DeploymentAction,
                                    IsBeta = updateInfo.IsBeta ? 1 : 0,
                                    IsDownloaded = updateInfo.IsDownloaded ? 1 : 0,
                                    IsHidden = updateInfo.IsHidden ? 1 : 0,
                                    IsInstalled = updateInfo.IsInstalled ? 1 : 0,
                                    IsMandatory = updateInfo.IsMandatory ? 1 : 0,
                                    InstallationRebootBehavior = updateInfo.InstallationRebootBehavior,
                                    IsUninstallable = updateInfo.IsUninstallable ? 1 : 0,
                                    ReleaseNotes = updateInfo.ReleaseNotes,
                                    UninstallationSteps = updateInfo.UninstallationSteps,
                                    UninstallationNotes = updateInfo.UninstallationNotes,
                                    UninstallationRebootBehavior = updateInfo.UninstallationRebootBehavior,
                                    Type = updateInfo.Type,
                                    SupportUrl = updateInfo.SupportUrl,
                                    SupersededUpdateIDs = string.Join(", ", updateInfo.SupersededUpdateIDs),
                                    SecurityBulletinIDs = string.Join(", ", updateInfo.SecurityBulletinIDs),
                                    BundledUpdates = string.Join(", ", updateInfo.BundledUpdates)
                                });
                        }


            _logger.LogInformation("Update check completed. Results saved to database.");

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection("Data Source=updates.db");
        connection.Execute(@"
    CREATE TABLE IF NOT EXISTS WindowsUpdateInfo (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        UpdateCheckTime TEXT,
        Title TEXT,
        Description TEXT,
        KBArticleIDs TEXT,
        Categories TEXT,
        DownloadSizeMB REAL,
        MoreInfoUrls TEXT,
        DatePublished TEXT,
        DeploymentAction TEXT,
        IsBeta INTEGER,
        IsDownloaded INTEGER,
        IsHidden INTEGER,
        IsInstalled INTEGER,
        IsMandatory INTEGER,
        InstallationRebootBehavior TEXT,
        IsUninstallable INTEGER,
        ReleaseNotes TEXT,
        UninstallationSteps TEXT,
        UninstallationNotes TEXT,
        UninstallationRebootBehavior TEXT,
        Type TEXT,
        SupportUrl TEXT,
        SupersededUpdateIDs TEXT,
        SecurityBulletinIDs TEXT,
        BundledUpdates TEXT
    )");
    }


    private List<WindowsUpdateInfo> CheckForUpdates()
    {
        List<WindowsUpdateInfo> updatesInfo = new List<WindowsUpdateInfo>();

        try
        {
            UpdateSession updateSession = new UpdateSession();
            IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();
            // ISearchResult searchResult = updateSearcher.Search("IsInstalled=0 And IsHidden=0");
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
                    Type = update.Type.ToString(), // https://learn.microsoft.com/en-us/windows/win32/api/wuapi/ne-wuapi-updatetype
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