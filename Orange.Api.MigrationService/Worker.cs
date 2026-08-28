using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orange.Api.utils;

namespace Orange.Api.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<Worker> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("[{Source}] Applying database migrations", "MigrationService");

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await dbContext.Database.MigrateAsync(stoppingToken);
            });

            logger.LogInformation("[{Source}] Database migrations applied", "MigrationService");
        }
        catch (Exception e)
        {
            logger.LogError(e, "[{Source}] Database migration failed", "MigrationService");
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }
}
