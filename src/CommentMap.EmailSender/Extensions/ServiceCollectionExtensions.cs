using System.Data.Common;

using CommentMap.EmailSender.Options;
using CommentMap.EmailSender.Services;

namespace CommentMap.EmailSender.Extensions;

public static class ServiceCollectionExtensions
{
    private const string DefaultConfigSectionName = "Aspire:Smtp";

    public static IHostApplicationBuilder AddSmtpEmailSenderServices(
        this IHostApplicationBuilder hostBuilder,
        string connectionName,
        Action<SmtpSettings>? configureSettings = null)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        ArgumentException.ThrowIfNullOrEmpty(connectionName, nameof(connectionName));

        SmtpSettings settings = new();

        var configSection = hostBuilder.Configuration.GetSection(DefaultConfigSectionName);
        configSection.Bind(settings);

        if (hostBuilder.Configuration.GetConnectionString(connectionName) is string connectionString)
        {
            var connectionBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString,
            };

            var smtpEndpoint = new Uri((string)connectionBuilder["Endpoint"], UriKind.Absolute);

            settings.Host = smtpEndpoint.Host;
            settings.Port = smtpEndpoint.Port;
        }

        configureSettings?.Invoke(settings);

        hostBuilder.Services.AddSingleton(Microsoft.Extensions.Options.Options.Create(settings));
        hostBuilder.Services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();
        hostBuilder.Services.AddSingleton<ISmtpEmailSender, SmtpEmailSender>();

        return hostBuilder;
    }
}
