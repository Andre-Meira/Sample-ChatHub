using RabbitMQ.Client;

namespace Sample.ChatHub.Bus;

public interface IConsumerContext<TMessage> where TMessage : class
{
    TMessage Message { get; init; }
    public ulong DeliveryTag { get; init; }
    public void NotifyConsumed();    
    public void NotifyFailConsumed(bool requeue = false);
}

internal record ConsumerContext<TMessage>(TMessage Message, ulong DeliveryTag, IModel Model)
    : IConsumerContext<TMessage> where TMessage : class
{
    public void NotifyConsumed() => Model.BasicAck(DeliveryTag, false);

    public void NotifyFailConsumed(bool requeue = false) => Model.BasicNack(DeliveryTag, false, requeue);
}

