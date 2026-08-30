using CommentMap.Application.Abstractions;
using CommentMap.Application.Entities;
using CommentMap.Infrastructure.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommentMap.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("comment-map");

        builder.Services
            .AddDbContext<ICommentMapDbContext, CommentMapDbContext>(options => options.UseNpgsql(
                connectionString,
                o => o.UseNetTopologySuite()))
            .AddDatabaseDeveloperPageExceptionFilter();

        builder.EnrichNpgsqlDbContext<CommentMapDbContext>();

        builder.Services
            .AddIdentity<User, Role>(options =>
            {
                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedAccount = true;
            })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<CommentMapDbContext>();

        return builder;
    }
}
