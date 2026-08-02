using CommentMap.Mvc.Data;
using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Extensions.DependencyInjection;
using CommentMap.Mvc.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using CommentMap.Shared.Messages;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Model;
using QRCoder;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddCommentMapDbContext();
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

builder.Services
    .AddIdentity<User, Role>(options =>
    {
        options.Stores.MaxLengthForKeys = 128;
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<CommentMapDbContext>();
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
builder.Services.AddSingleton<IEnableAuthenticatorService, EnableAuthenticatorService>();
builder.Services.AddScoped<IListCommentsService, ListCommentsService>();
builder.Services.AddScoped<ICommentFactory, CommentFactory>();
builder.Services.AddScoped<IAddCommentService, AddCommentService>();
builder.Services.AddScoped<IDeleteCommentService, DeleteCommentService>();
builder.Services.AddScoped<IConfirmDeleteService, ConfirmDeleteService>();
builder.Services.AddScoped<IGetCountryViewModelService, GetCountryViewModelService>();
builder.Services.AddScoped<IGuessCountryService, GuessCountryService>();


var mvcBuilder = builder.Services.AddRazorPages();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        var configuration = ctx.GetRequiredService<IConfiguration>();
        var host = configuration.GetConnectionString("messaging");
        cfg.Host(host);
    });
});
builder.Services.AddRazorPages();

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
