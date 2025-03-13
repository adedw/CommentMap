using CommentMap.Mvc.Data;
using CommentMap.Mvc.Data.Entities;
using CommentMap.Mvc.Extensions.DependencyInjection;
using CommentMap.Mvc.Services;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using QRCoder;

var builder = WebApplication.CreateBuilder(args);


builder.AddCommentMapDbContext();

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