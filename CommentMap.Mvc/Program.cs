using CommentMap.Mvc.Data;
using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Extensions.DependencyInjection;
using CommentMap.Mvc.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    // Add services to the container.
    builder.Services.AddSerilog();

    builder.Services.AddCommentMapDbContext("DefaultConnection");

    builder.Services
        .AddDefaultIdentity<User>()
        .AddEntityFrameworkStores<CommentMapDbContext>();
    builder.Services.Configure<IdentityOptions>(options => options.Lockout.AllowedForNewUsers = false);
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

    var mvcBuilder = builder.Services.AddRazorPages();

    if (builder.Environment.IsDevelopment())
    {
        mvcBuilder.AddRazorRuntimeCompilation();
    }

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