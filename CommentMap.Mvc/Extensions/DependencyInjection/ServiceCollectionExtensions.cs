using CommentMap.Mvc.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CommentMap.Mvc.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommentMapDbContext(this IServiceCollection serviceCollection, string connectionStringKey)
    {
        return serviceCollection
            .AddDbContext<ICommentMapDbContext, CommentMapDbContext>((provider, optionsBuilder) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString(connectionStringKey);

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                dataSourceBuilder.UseNetTopologySuite();
                var dataSource = dataSourceBuilder.Build();

                optionsBuilder.UseNpgsql(dataSource, npgsqlOptions => npgsqlOptions.UseNetTopologySuite());
            })
            .AddDatabaseDeveloperPageExceptionFilter();
    }
}
