using RabbitMQ.Client;
using Sample.ChatHub.Bus.Extesions;

namespace Sample.ChatHub.Bus;

public interface IPublishContext
{
    public Task PublishMessage<TMessage>(TMessage message, string routingKey = "", TimeSpan timeout = default) where TMessage : class;
}


internal class PublishContext : IPublishContext, IDisposable
{
    private readonly IModel _channel;

    public PublishContext(IConnection connection) => _channel = connection.CreateModel();

    public void Dispose() => _channel.Dispose();

    public Task PublishMessage<TMessage>(TMessage message, string routingKey = "", TimeSpan timeout = default) 
        where TMessage : class
    {
        timeout = timeout == default ? TimeSpan.FromSeconds(5) : timeout;
        return _channel.PublishConfirmMessage(message, timeout, routingKey);    
    }
}