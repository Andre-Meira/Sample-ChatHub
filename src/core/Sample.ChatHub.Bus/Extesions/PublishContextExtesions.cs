using Newtonsoft.Json;
using RabbitMQ.Client;
using Sample.ChatHub.Bus.Monitory;
using System.Diagnostics;
using System.Text;

namespace Sample.ChatHub.Bus.Extesions;

internal static class PublishContextExtesions
{
    public static Task PublishMessageAsync<TMessage>(this IModel model, TMessage message, string routingKey = "")
        where TMessage : class
    {
        string exchange = MessageExtesions.GetExchangeContract<TMessage>();
        string exchageType = MessageExtesions.GetExchangeTypeContract<TMessage>();

        model.ExchangeDeclare(exchange, exchageType, true);
        byte[] body = message.SerializationMessage();

        return Task.Run(() => model.BasicPublish(exchange: exchange, routingKey, GetProperties(model), body));
    }

    public static async Task PublishConfirmMessage<TMessage>(
        this IModel model,
        TMessage message,
        TimeSpan timeout = default,

        string routingKey = "")
        where TMessage : class
    {
        ActivityBus? activityBus = message.CreatePublishActivityBus(routingKey);
        activityBus?.Start();

        string exchange = MessageExtesions.GetExchangeContract<TMessage>();
        string exchageType = MessageExtesions.GetExchangeTypeContract<TMessage>();

        model.ConfirmSelect();
        try
        {
            timeout = timeout == default ? TimeSpan.FromSeconds(5) : timeout;

            model.ExchangeDeclare(exchange, exchageType, true);
            byte[] body = message.SerializationMessage();

            await Task.Run(() =>
            {
                model.BasicPublish(exchange: exchange, routingKey, GetProperties(model), body);
                model.WaitForConfirmsOrDie(timeout);
            });
        }
        catch (Exception err)
        {
            activityBus?.AddExceptionEvent(err);
            throw;
        }
        finally
        {
            activityBus?.Stop();
        }
    }


    public static IBasicProperties GetProperties(this IModel model)
    {
        IBasicProperties properties = model.CreateBasicProperties();
        properties.DeliveryMode = 2;
        properties.ContentType = "application/json";

        properties.Headers = new Dictionary<string, object?>()
        {
            { ActivityConstants.Header_Id,  Activity.Current?.Id?.ToString() },
            { ActivityConstants.Header_TraceId,  Activity.Current?.TraceId.ToString() },
            { ActivityConstants.Header_SpanId, Activity.Current?.Context.SpanId.ToString() },
            { ActivityConstants.Header_TraceFlags, Activity.Current?.Context.TraceFlags.ToString() },
            { ActivityConstants.Header_TraceState, Activity.Current?.Context.TraceState?.ToString() }
        };

        return properties;
    }

}