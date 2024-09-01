﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenWindowsPatchMan.Agent.Core;
using OpenWindowsPatchMan.Agent.Core.Database;
using OpenWindowsPatchMan.Agent.Core.Services;


namespace OpenWindowsPatchMan.Agent.Service;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<PatchManWorkerService>();

                // Register IDbContextFactory for PatchManDbContext
                services.AddDbContextFactory<PatchManDbContext>(options =>
                    options.UseSqlite(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                // Register services
                services.AddSingleton<IPatchManUpdateService, PatchManUpdateService>();
                services.AddSingleton<IPatchManDatabaseService, PatchManDatabaseService>();
            });
}