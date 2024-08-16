using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenWindowsPatchMan.Agent.Core.Models;


namespace OpenWindowsPatchMan.Agent.Service;

public class PatchManDatabaseService : IPatchManDatabaseService
{
    private readonly ILogger<PatchManDatabaseService> _logger;
    private readonly IDbContextFactory<UpdateContext> _contextFactory;

    public PatchManDatabaseService(ILogger<PatchManDatabaseService> logger, IDbContextFactory<UpdateContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public void InitializeDatabase()
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            // Ensure the database is created
            context.Database.EnsureCreated();
        }
    }

    public void SaveUpdateInfo(List<WindowsUpdateInfo> updatesInfo)
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            context.WindowsUpdateInfos.AddRange(updatesInfo);
            context.SaveChanges();
            _logger.LogInformation("Update check results saved to database.");
        }
    }

    public void SaveInstallationResults(List<WindowsUpdateInfo> updatesInfo)
    {
        using (var context = _contextFactory.CreateDbContext())
        {
            foreach (var updateInfo in updatesInfo)
            {
                var existingUpdate = context.WindowsUpdateInfos
                    .FirstOrDefault(u => u.Title == updateInfo.Title && u.DatePublished == updateInfo.DatePublished);

                if (existingUpdate != null)
                {
                    //existingUpdate.InstallationResultCode = updateInfo.InstallationResultCode;
                    //existingUpdate.RebootRequired = updateInfo.RebootRequired;
                }
            }

            context.SaveChanges();
            _logger.LogInformation("Installation results saved to database.");
        }
    }
}