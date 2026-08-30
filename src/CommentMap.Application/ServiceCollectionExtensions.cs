using System.Reflection;

using CommentMap.Application.Abstractions;
using CommentMap.Application.Features.Identity;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommentMap.Application;

public static class ServiceCollectionExtensions
{
    internal static readonly Assembly Assembly = typeof(ServiceCollectionExtensions).Assembly;

    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
        => AddMessageHandlers(services, Assembly);

    public static IServiceCollection AddMessageHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t is { IsAbstract: false, IsInterface: false })
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                         i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                         i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                    .Select(i => (ServiceType: i, ImplementationType: t)));

            foreach (var (serviceType, implementationType) in handlerTypes)
            {
                services.AddTransient(serviceType, implementationType);
            }
        }

        services.TryAddTransient<IAuthenticatorSetupProvider, AuthenticatorSetupProvider>();
        return services;
    }
}
