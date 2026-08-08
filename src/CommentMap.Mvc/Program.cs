using CommentMap.Application.Features.Comments;
using CommentMap.Infrastructure.DependencyInjection;
using CommentMap.Shared.Messages;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Model;
using QRCoder;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseWolverine(opts =>
{
    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
    opts.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
    opts.Discovery.IncludeAssembly(typeof(AddComment).Assembly);

    opts.UseRabbitMqUsingNamedConnection("messaging")
        .AutoProvision();

    opts.PublishMessage<SendConfirmEmail>().ToRabbitQueue(nameof(SendConfirmEmail));
    opts.PublishMessage<SendResetPasswordEmail>().ToRabbitQueue(nameof(SendResetPasswordEmail));
    opts.PublishMessage<SendChangeEmail>().ToRabbitQueue(nameof(SendChangeEmail));
});

builder.AddInfrastructure();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
builder.Services
    .AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });
builder.Services.AddSingleton<QRCodeGenerator>();

var mvcBuilder = builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

return await app.RunJasperFxCommands(args);
