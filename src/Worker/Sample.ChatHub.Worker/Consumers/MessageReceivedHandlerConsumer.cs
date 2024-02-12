using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Worker.Core.Messages.Events;

namespace Sample.ChatHub.Worker.Consumers;

internal class MessageReceivedHandlerConsumer : ConsumerHandlerBase<MessageReceived>
{
    private readonly IMessageProcessStream _messageProcess;

    public MessageReceivedHandlerConsumer(
        IConnectionFactory connectionFactory,
        IMessageProcessStream messageProcess)
    : base(connectionFactory)
    {
        this.ExchageType = ExchangeType.Direct;
        this.PrefetchCount = 20;
        this.ExchangeName = "message-received-consumer";
        _messageProcess = messageProcess;
    }

    public override async Task Consumer(IConsumerContext<MessageReceived> context)
    {
        var @event = new ReceivedMessage(context.Message.IdChat, context.Message.IdMessage, 
            context.Message.IdUser);

        await _messageProcess.Include(@event).ConfigureAwait(false);
        context.NotifyConsumed();
    }
}
