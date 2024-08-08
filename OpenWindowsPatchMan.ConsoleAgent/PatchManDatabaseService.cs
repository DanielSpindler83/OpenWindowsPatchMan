using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenWindowsPatchMan.ConsoleAgent;

public class PatchManDatabaseService : IPatchManDatabaseService
{
    private readonly ILogger<PatchManDatabaseService> _logger;
    private readonly string _connectionString;

    public PatchManDatabaseService(ILogger<PatchManDatabaseService> logger, IConfiguration configuration)
    {
        _logger = logger;
        //_connectionString = configuration.GetConnectionString("DefaultConnection");
        _connectionString = "Data Source=updates.db";
    }

    public void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
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

    public void SaveUpdateInfo(List<WindowsUpdateInfo> updatesInfo)
    {
        using var connection = new SqliteConnection(_connectionString);
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

        _logger.LogInformation("Update check results saved to database.");
    }
}