using RabbitMQ.Client;

namespace Sample.ChatHub.Bus;

public interface IConsumerContext<TMessage> where TMessage : class
{    
    TMessage Message { get; init;}
    public ulong DeliveryTag { get; init;  }        
    public void NotifyConsumed();
}

internal record ConsumerContext<TMessage>(TMessage Message, ulong DeliveryTag, IModel Model) 
    : IConsumerContext<TMessage> where TMessage : class
{    
    public void NotifyConsumed() => Model.BasicAck(DeliveryTag, false);
    
}

