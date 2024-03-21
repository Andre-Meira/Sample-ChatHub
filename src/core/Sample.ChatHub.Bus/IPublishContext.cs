using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Sample.ChatHub.Bus;

public interface IPublishContext
{
    public Task PublishMessage<TMessage>(TMessage message, string routingKey = "")
        where TMessage : class;
}


internal class PublishContext : IPublishContext, IDisposable
{
    private readonly IModel _channel;

    public PublishContext(IConnection connection)
    {
        _channel = connection.CreateModel();
    }

    public void Dispose() => _channel.Dispose();

    public Task PublishMessage<TMessage>(TMessage message, string routingKey = "") where TMessage : class
    {
        string exchange = ContractExtensions.GetExchangeContract<TMessage>();
        string exchageType = ContractExtensions.GetExchangeTypeContract<TMessage>();

        _channel.ExchangeDeclare(exchange, exchageType, true);

        IBasicProperties properties = _channel.CreateBasicProperties();
        properties.DeliveryMode = 2;
        properties.ContentType = "application/json";

        string json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        return Task.Run(() => _channel.BasicPublish(exchange: exchange, routingKey, properties, body));
    }
}