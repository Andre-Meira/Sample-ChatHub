using OpenTelemetry.Trace;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.ChatHub.Bus.Extesions;
using System.Diagnostics;
using System.Net;

namespace Sample.ChatHub.Bus.Monitory;

public static class ActivityTraceExntesions
{
    private static readonly ActivitySource activitySource = new ActivitySource(SourceName, SourceVersion);

    public const string SourceName = "MessageBroker";
    public const string SourceVersion = "1.0.0";


    public static TracerProviderBuilder AddTracingServiceBus(this TracerProviderBuilder tracerProvider)
    {       
        return tracerProvider.AddSource("MessageBroker");
    }

    public static ActivityBus? CreatePublishActivityBus<TMessage>(this TMessage message, string routingKey)
        where TMessage : class
    {
        var activity = Activity.Current;
        if (activity is null) return null;            

        var customActivity = activitySource.CreateActivity("PublishMessage", ActivityKind.Producer, activity.Context);
        
        if (customActivity == null) return null;

        string exchange = MessageExtesions.GetExchangeContract<TMessage>();
        string exchageType = MessageExtesions.GetExchangeTypeContract<TMessage>();

        customActivity.SetTagDefault();
        customActivity.SetTag("message.name", nameof(TMessage));
        customActivity.SetTag("message.routing", routingKey);
        customActivity.SetTag("message.exchange", exchange);
        customActivity.SetTag("message.exchageType", exchageType);        

        return new ActivityBus(customActivity);        
    }

    public static ActivityBus? CreateConsumerActivityBus(this BasicDeliverEventArgs message)
    {
        var headers = message.BasicProperties.Headers;

        if (headers.TryGetValue("activity.id", out var headerValue)
            && headerValue is string activityId
            && ActivityContext.TryParse(activityId, null, out var activityContext))
        {
            var parentActivityContext = new ActivityContext(activityContext.TraceId, activityContext.SpanId,
                activityContext.TraceFlags, activityContext.TraceState, true);

            var customActivity = activitySource.CreateActivity("ConsumerMessage", ActivityKind.Consumer, parentActivityContext);
            customActivity?.SetTagDefault();

            if (customActivity == null) return null;

            return new ActivityBus(customActivity);
        }

        return default;        
    }


    private static void SetTagDefault(this Activity activity)
    {
        activity.SetTag("hostname", Dns.GetHostName());
        activity.SetTag("activity.id", activity.Id);
    }
}
