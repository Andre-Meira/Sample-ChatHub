using OpenTelemetry.Trace;
using RabbitMQ.Client.Events;
using Sample.ChatHub.Bus.Extesions;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Sample.ChatHub.Bus.Monitory;

public static class ActivityTraceExntesions
{
    private static readonly ActivitySource activitySource = new ActivitySource(ActivityConstants.Source, "1.0.0");
   
    public static TracerProviderBuilder AddTracingServiceBus(this TracerProviderBuilder tracerProvider)
    {
        return tracerProvider.AddSource(ActivityConstants.Source);
    }

    public static ActivityBus? CreatePublishActivityBus<TMessage>(this TMessage message, string routingKey)
        where TMessage : class
    {
        var activity = Activity.Current;
        if (activity is null) return null;

        var customActivity = activitySource.CreateActivity(ActivityConstants.NameActivity_Publish, ActivityKind.Producer, activity.Context);

        if (customActivity == null) return null;

        string exchange = MessageExtesions.GetExchangeContract<TMessage>();
        string exchageType = MessageExtesions.GetExchangeTypeContract<TMessage>();

        customActivity.SetTagDefault();
        customActivity.SetTag(ActivityConstants.MessageName, typeof(TMessage).Name);
        customActivity.SetTag(ActivityConstants.MessageRouting, routingKey);
        customActivity.SetTag(ActivityConstants.MessageExchange, exchange);
        customActivity.SetTag(ActivityConstants.MessageExchageType, exchageType);

        return new ActivityBus(customActivity);
    }

    public static ActivityBus? CreateConsumerActivityBus(this BasicDeliverEventArgs message)
    {
        var headers = message.BasicProperties.Headers;
        var value = headers[ActivityConstants.Header_Id];

        if (value == null) return null;
        string valueString = Encoding.UTF8.GetString((byte[])value);


        if (ActivityContext.TryParse(valueString, null, out var activityContext))
        {
            var parentActivityContext = new ActivityContext(activityContext.TraceId, activityContext.SpanId,
                activityContext.TraceFlags, activityContext.TraceState, true);

            var customActivity = activitySource.CreateActivity(ActivityConstants.NameActivity_Consumer, ActivityKind.Consumer, parentActivityContext);
            customActivity?.SetTagDefault();

            if (customActivity == null) return null;

            return new ActivityBus(customActivity);
        }

        return default;
    }


    private static void SetTagDefault(this Activity activity)
    {
        activity.SetTag(ActivityConstants.Hostname, Dns.GetHostName());
        activity.SetTag(ActivityConstants.TraceId, activity.TraceId);
        activity.SetTag(ActivityConstants.SpanId, activity.SpanId);
    }
}
