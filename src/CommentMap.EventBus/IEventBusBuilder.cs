using Microsoft.Extensions.DependencyInjection;

namespace CommentMap.EventBus;

public interface IEventBusBuilder
{
    IServiceCollection Services { get; }
}
