using CommentMap.EmailSender.Options;
using CommentMap.EmailSender.Services;
using MailKit.Net.Smtp;

namespace CommentMap.EmailSender.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmtpEmailSenderServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISmtpClient, SmtpClient>();

        var mailpitServiceConfiguration = configuration.GetSection(MailpitServiceOptions.SectionName);
        services.AddOptions<MailpitServiceOptions>()
            .Bind(mailpitServiceConfiguration)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<ISmtpEmailSenderService, SmtpEmailSenderService>();

        return services;
    }
}
