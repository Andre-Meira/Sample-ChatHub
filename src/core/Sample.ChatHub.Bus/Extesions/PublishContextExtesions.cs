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
        string exchange = "", 
        string exchageType = "", 
        string routingKey = "")
        where TMessage : class
    {
        ActivityBus? activityBus = message.CreatePublishActivityBus(routingKey);
        activityBus?.Start();

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

            activityBus?.Stop();
        }
        catch (Exception err)
        {
            activityBus?.AddExceptionEvent(err);
            throw;
        }
    }
        

    public static IBasicProperties GetProperties(this IModel model)
    {
        IBasicProperties properties = model.CreateBasicProperties();
        properties.DeliveryMode = 2;
        properties.ContentType = "application/json";
        
        properties.Headers = new Dictionary<string, object?>()
        {
            { "activity_trace_Id",  Activity.Current?.Context.TraceId },
            { "activity_parent_spanId", Activity.Current?.Context.SpanId },
            { "activity_parent_traceFlags", Activity.Current?.Context.TraceFlags},
            { "activity_parent_traceState", Activity.Current?.Context.TraceState}
        };

        return properties;
    }

}