using System.Data;
using System.Diagnostics;

using CommentMap.Infrastructure.Data;
using CommentMap.MigrationService.Logging;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace CommentMap.MigrationService;

public class Migrator(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<Migrator> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CommentMapDbContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
            await SeedCountriesAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private async Task RunMigrationAsync(CommentMapDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            logger.LogApplyingMigrations();
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogMigrationsApplied();
        });
    }

    private async Task SeedCountriesAsync(CommentMapDbContext dbContext, CancellationToken cancellationToken)
    {
        var connection = (NpgsqlConnection)dbContext.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        const string countSql = """SELECT COUNT(*) FROM "Countries";""";
        await using var countCommand = new NpgsqlCommand(countSql, connection);
        var existing = (long)(await countCommand.ExecuteScalarAsync(cancellationToken))!;

        if (existing > 0)
        {
            logger.LogCountriesSeedSkipped(existing);
            return;
        }

        var assembly = typeof(Migrator).Assembly;
        const string resourceName = "CommentMap.MigrationService.Seed.countries.sql";
        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found");
        using var reader = new StreamReader(stream);
        var script = await reader.ReadToEndAsync(cancellationToken);

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            try
            {
                logger.LogSeedingCountries();
                await using var command = new NpgsqlCommand(script, connection, transaction);
                var affected = await command.ExecuteNonQueryAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                logger.LogCountriesSeeded(affected);
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        });
    }
}
