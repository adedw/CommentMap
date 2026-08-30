using Microsoft.Extensions.Logging;

namespace CommentMap.MigrationService.Logging;

internal static partial class MigratorLogs
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "Applying database migrations...")]
    public static partial void LogApplyingMigrations(this ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information,
        Message = "Database migrations applied successfully")]
    public static partial void LogMigrationsApplied(this ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information,
        Message = "Countries seed skipped: {Count} rows already present")]
    public static partial void LogCountriesSeedSkipped(this ILogger logger, long count);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information,
        Message = "Seeding countries...")]
    public static partial void LogSeedingCountries(this ILogger logger);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information,
        Message = "Countries seeded: {Count} rows")]
    public static partial void LogCountriesSeeded(this ILogger logger, long count);
}
