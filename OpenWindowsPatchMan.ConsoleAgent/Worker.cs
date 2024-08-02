

using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WUApiLib;

namespace OpenWindowsPatchMan.ConsoleAgent;

// https://learn.microsoft.com/en-us/windows/win32/api/_wua/

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private TimeSpan _checkInterval;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
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
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            List<WindowsUpdateInfo> updatesInfo = CheckForUpdates();

            using var connection = new SqliteConnection("Data Source=updates.db");
            foreach (var update in updatesInfo)
            {
                connection.Execute(@"
                    INSERT INTO WindowsUpdateInfo (Title, Description, KBArticleIDs, Categories, DownloadSizeMB, MoreInfoUrls)
                    VALUES (@Title, @Description, @KBArticleIDs, @Categories, @DownloadSizeMB, @MoreInfoUrls)",
                    new
                    {
                        Title = update.Title,
                        Description = update.Description,
                        KBArticleIDs = string.Join(", ", update.KBArticleIDs),
                        Categories = string.Join(", ", update.Categories),
                        DownloadSizeMB = update.DownloadSizeMB,
                        MoreInfoUrls = string.Join(", ", update.MoreInfoUrls)
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
            Title TEXT,
            Description TEXT,
            KBArticleIDs TEXT,
            Categories TEXT,
            DownloadSizeMB REAL,
            MoreInfoUrls TEXT
        )");
    }


    private List<WindowsUpdateInfo> CheckForUpdates()
    {
        List<WindowsUpdateInfo> updatesInfo = new List<WindowsUpdateInfo>();

        try
        {
            UpdateSession updateSession = new UpdateSession();
            IUpdateSearcher updateSearcher = updateSession.CreateUpdateSearcher();
            ISearchResult searchResult = updateSearcher.Search("IsInstalled=0");

            foreach (IUpdate update in searchResult.Updates)
            {
                double downloadSizeMB = Math.Round((double)update.MaxDownloadSize / 1024.0 / 1024.0, 2);

                WindowsUpdateInfo updateInfo = new WindowsUpdateInfo
                {
                    Title = update.Title,
                    Description = update.Description,
                    DownloadSizeMB = downloadSizeMB
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