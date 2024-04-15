using Newtonsoft.Json;
using RabbitMQ.Client;
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

    public static Task PublishConfirmMessage<TMessage>(this IModel model, TMessage message, TimeSpan timeout, string routingKey = "")
        where TMessage : class
    {
        model.CreateBasicProperties();

        string exchange = MessageExtesions.GetExchangeContract<TMessage>();
        string exchageType = MessageExtesions.GetExchangeTypeContract<TMessage>();

        model.ExchangeDeclare(exchange, exchageType, true);
        byte[] body = message.SerializationMessage();

        return Task.Run(() =>
        {
            model.BasicPublish(exchange: exchange, routingKey, GetProperties(model), body);
            model.WaitForConfirmsOrDie(timeout);
        });
    }

    public static Task PublishConfirmMessage<TMessage>(this IModel model, TMessage message, 
        TimeSpan timeout = default, string exchange = "", string exchageType = "", string routingKey = "")
        where TMessage : class
    {
        model.CreateBasicProperties();

        timeout = timeout == default ? TimeSpan.FromSeconds(5) : timeout;

        model.ExchangeDeclare(exchange, exchageType, true);
        byte[] body = message.SerializationMessage();

        return Task.Run(() =>
        {
            model.BasicPublish(exchange: exchange, routingKey, GetProperties(model), body);
            model.WaitForConfirmsOrDie(timeout);
        });
    }


    public static IBasicProperties GetProperties(this IModel model)
    {
        IBasicProperties properties = model.CreateBasicProperties();
        properties.DeliveryMode = 2;
        properties.ContentType = "application/json";

        return properties;
    }
}