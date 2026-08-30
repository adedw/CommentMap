using CommentMap.EmailSender.Extensions;
using CommentMap.EmailSender.Features;
using CommentMap.EmailSender.Services;
using CommentMap.EventBus;
using CommentMap.Shared.Messages;

using Mjml.Net;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddSmtpEmailSenderServices(connectionName: "mailpit");
builder.Services.AddSingleton<IMjmlRenderer>(new MjmlRenderer());
builder.Services.AddSingleton<IMessageSenderService, MessageSenderService>();

builder.AddEventBus("messaging")
    .AddSubscription<SendConfirmEmail, SendConfirmEmailHandler>()
    .AddSubscription<SendResetPasswordEmail, SendResetPasswordEmailHandler>()
    .AddSubscription<SendChangeEmail, SendChangeEmailHandler>();

var host = builder.Build();
host.Run();
