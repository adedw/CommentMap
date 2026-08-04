using CommentMap.Infrastructure.DependencyInjection;
using CommentMap.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddInfrastructure();

builder.Services.AddHostedService<Migrator>();

var host = builder.Build();
host.Run();
