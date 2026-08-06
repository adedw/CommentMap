using CommentMap.EmailSender.Extensions;
using CommentMap.EmailSender.Services;
using CommentMap.Shared.Messages;
using JasperFx;
using JasperFx.CodeGeneration;
using Mjml.Net;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddSmtpEmailSenderServices(connectionName: "mailpit");
builder.Services.AddSingleton<IMjmlRenderer>(new MjmlRenderer());
builder.Services.AddSingleton<IMessageSenderService, MessageSenderService>();

builder.UseWolverine(opts =>
{
    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;

    opts.UseRabbitMqUsingNamedConnection("messaging")
        .AutoProvision();

    opts.ListenToRabbitQueue(nameof(SendConfirmEmail));
    opts.ListenToRabbitQueue(nameof(SendResetPasswordEmail));
    opts.ListenToRabbitQueue(nameof(SendChangeEmail));
});

var host = builder.Build();
return await host.RunJasperFxCommands(args);
