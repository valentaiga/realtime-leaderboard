using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Migrations.Postgres;

public static class MigrationsDiExtensions
{
    public static IServiceCollection AddMigrationsDbContext<TDbContext>(this IServiceCollection services, IConfiguration configuration, string connectionStringName) where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString(connectionStringName)
                                   ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not configured");

            sp.GetRequiredService<ILogger<TDbContext>>()
                .LogInformation("Current connection is '{ConnectionString}'", connectionString);
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("migrations_history");
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                npgsqlOptions.CommandTimeout(30);
            });
        });

        return services;
    }

    public static async Task EnsureMigrationAsync<TDbContext>(this IServiceProvider serviceProvider) where TDbContext : DbContext
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TDbContext>>();

        try
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations");
            throw;
        }
    }
}
