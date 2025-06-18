using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Data.SeedData;

namespace Ordering.Infrastructure.Data.Extensions;
public static class DatabaseExtentions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        // Retry logic for database connection
        var maxRetries = 30;
        var retryDelay = TimeSpan.FromSeconds(2);

        for (int retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                logger.LogInformation("🔄 Attempting to connect to Ordering SQL Server (attempt {Retry}/{MaxRetries})", retry + 1, maxRetries);

                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Migrate database
                await context.Database.MigrateAsync();

                // Seed data
                await SeedAsync(context, logger);

                logger.LogInformation("✅ Ordering database initialization completed successfully");
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning("⚠️ Ordering database connection failed (attempt {Retry}/{MaxRetries}): {Error}",
                    retry + 1, maxRetries, ex.Message);

                if (retry == maxRetries - 1)
                {
                    logger.LogError("❌ Failed to connect to Ordering SQL Server after {MaxRetries} attempts", maxRetries);
                    throw;
                }

                await Task.Delay(retryDelay);
            }
        }
    }

    private static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        // Use only the new comprehensive seed data that matches Identity Service users
        await OrderingSeedData.SeedAsync(context, logger);
    }
}
