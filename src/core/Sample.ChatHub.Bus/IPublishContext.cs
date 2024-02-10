using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Sample.ChatHub.Bus;

public interface IPublishContext
{
    public Task PublishMessage<TMessage>(TMessage message, string routingKey = "") 
        where TMessage : class;
}


internal class PublishContext : IPublishContext, IDisposable
{
    private readonly IModel _channel;

    public PublishContext(IConnectionFactory connectionFactory)
    {
        IConnection connection = connectionFactory.CreateConnection();
        _channel = connection.CreateModel();
    }


    public void Dispose() => _channel.Dispose();
    
    public Task PublishMessage<TMessage>(TMessage message, string routingKey = "") where TMessage : class
    {
        string exchange = ContractExtensions.GetExchangeContract<TMessage>();
        string exchageType = ContractExtensions.GetExchangeTypeContract<TMessage>();

        _channel.ExchangeDeclare(exchange, exchageType, true);

        string json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        return Task.Run(() => _channel.BasicPublish(exchange: exchange, routingKey, null, body));
    }
}