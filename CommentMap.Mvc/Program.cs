using CommentMap.Mvc.Data;
using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Extensions.DependencyInjection;
using CommentMap.Mvc.Models;
using CommentMap.Mvc.Services;
using Microsoft.AspNetCore.Identity;
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
            options.SignIn.RequireConfirmedAccount = false;
            options.Lockout.AllowedForNewUsers = false;
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
            var googleAuthenticationOptions = builder.Configuration
                .GetSection("Authentication:Google")
                .Get<GoogleAuthenticationOptions>();
            googleOptions.ClientId = googleAuthenticationOptions.ClientId;
            googleOptions.ClientSecret = googleAuthenticationOptions.ClientSecret;
        });
    builder.Services.AddSingleton<QRCodeGenerator>();
    builder.Services.AddSingleton<IEnableAuthenticatorService, EnableAuthenticatorService>();

    builder.Services.AddScoped<IListCommentsService, ListCommentsService>();
    builder.Services.AddSingleton<IIdGenerationService, UuidV7GenerationService>();
    builder.Services.AddSingleton<ICommentFactory, CommentFactory>();
    builder.Services.AddScoped<IAddCommentService, AddCommentService>();

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
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapRazorPages();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}