using CommentMap.Mvc.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CommentMap.Mvc.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddCommentMapDbContext(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("comment-map");

        builder.Services
            .AddDbContext<ICommentMapDbContext, CommentMapDbContext>(options => options.UseNpgsql(
                connectionString,
                o => o.UseNetTopologySuite()))
            .AddDatabaseDeveloperPageExceptionFilter();

        return builder;
    }
}
