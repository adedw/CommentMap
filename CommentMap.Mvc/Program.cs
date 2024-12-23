using CommentMap.Mvc.Data;
using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Extensions.DependencyInjection;
using CommentMap.Mvc.Models;
using CommentMap.Mvc.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using QRCoder;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    // Add services to the container.
    builder.Services.AddSerilog();

    var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddCommentMapDbContext(defaultConnectionString);

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
    builder.Services.AddOptions<SmtpEmailSenderOptions>()
        .Bind(builder.Configuration.GetSection(nameof(SmtpEmailSenderOptions.SmtpEmailSender)))
        .ValidateDataAnnotations()
        .ValidateOnStart();
    builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

    var mvcBuilder = builder.Services.AddRazorPages();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
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

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
