using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Worker.Core.Messages.Events;

namespace Sample.ChatHub.Worker.API.Consumers;

internal sealed class SendMessageHandlerConsumer : ConsumerHandlerBase<SendMessage>
{
    private readonly IMessageProcessStream _messageProcess;

    public SendMessageHandlerConsumer(
        IConnectionFactory connectionFactory,
        IMessageProcessStream messageProcess) 
    : base(connectionFactory)
    {
        this.ExchageType = ExchangeType.Direct;
        this.PrefetchCount = 20;
        this.ExchangeName = "send-message-consumer";
        _messageProcess = messageProcess;
    }

    public override async Task Consumer(IConsumerContext<SendMessage> context)
    {
        var @event = new SendMessageChat(context.Message.Context);

        await _messageProcess.Include(@event).ConfigureAwait(false);
        context.NotifyConsumed();
    }
}
