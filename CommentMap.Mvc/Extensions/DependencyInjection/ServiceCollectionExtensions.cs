using CommentMap.Mvc.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CommentMap.Mvc.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommentMapDbContext(this IServiceCollection serviceCollection, string? connectionString = null)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseNetTopologySuite();
        var dataSource = dataSourceBuilder.Build();

        return serviceCollection
            .AddDbContext<ICommentMapDbContext, CommentMapDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(dataSource, npgsqlOptions => npgsqlOptions.UseNetTopologySuite()))
            .AddDatabaseDeveloperPageExceptionFilter();
    }
}
