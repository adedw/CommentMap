using CommentMap.EmailSender.Extensions;
using CommentMap.EmailSender.Services;
using CommentMap.Shared.Messages;
using MassTransit;
using Mjml.Net;


var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSmtpEmailSenderServices(builder.Configuration);
builder.Services.AddScoped<IMjmlRenderer>(_ => new MjmlRenderer());
builder.Services.AddScoped<IMessageSenderService, MessageSenderService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageSenderConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration.GetConnectionString("messaging");
        cfg.Host(host);

        cfg.ReceiveEndpoint(nameof(SendChangeEmail), endpoint =>
        {
            endpoint.ConfigureConsumer<MessageSenderConsumer>(context);
        });
        cfg.ReceiveEndpoint(nameof(SendConfirmEmail), endpoint =>
        {
            endpoint.ConfigureConsumer<MessageSenderConsumer>(context);
        });
        cfg.ReceiveEndpoint(nameof(SendResetPasswordEmail), endpoint =>
        {
            endpoint.ConfigureConsumer<MessageSenderConsumer>(context);
        });
    });
});

builder.Build().Run();
