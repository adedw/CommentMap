using System.Diagnostics;

using OpenTelemetry.Context.Propagation;

namespace CommentMap.EventBus;

public class RabbitMQTelemetry
{
    internal const string ActivitySourceName = "CommentMap.EventBus";

    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    public ActivitySource ActivitySource => _activitySource;

    public TextMapPropagator Propagator => Propagators.DefaultTextMapPropagator;

    public void Dispose() => _activitySource.Dispose();
}
