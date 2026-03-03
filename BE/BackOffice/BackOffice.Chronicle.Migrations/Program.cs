using BackOffice.Chronicle.Migrations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// builder.
builder.Services
    .AddDbContext<ChronicleDbContext>((sp, options) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("ChronicleDb")
                               ?? throw new InvalidOperationException("Connection string 'ChronicleDb' not configured");

        sp.GetRequiredService<ILogger<ChronicleDbContext>>()
            .LogInformation("Current connection is '{ConnectionString}'", connectionString);
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("migrations_history");
            npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            npgsqlOptions.CommandTimeout(30);
        });
    });

var app = builder.Build();

await EnsureMigrationAsync(app.Services);

return;

static async Task EnsureMigrationAsync(IServiceProvider serviceProvider)
{
    await using var scope = serviceProvider.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<ChronicleDbContext>();

    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ChronicleDbContext>>();
        logger.LogError(ex, "Failed to apply migrations");
        throw;
    }
}