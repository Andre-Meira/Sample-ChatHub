using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Core.Chat.Events;
using Sample.ChatHub.Domain.Contracts.Messages;

namespace Sample.ChatHub.Worker.Consumers;

internal sealed class SendMessageHandlerConsumer : ConsumerHandlerBase<SendMessage>
{
    private readonly IChatProcessStream _chatProcess;

    public SendMessageHandlerConsumer(
        IConnectionFactory connectionFactory, 
        IChatProcessStream chatProcess) 
    : base(connectionFactory)
    {
        this.ExchageType = ExchangeType.Direct;
        this.PrefetchCount = 20;
        this.ExchangeName = "send-message-consumer";
        _chatProcess = chatProcess;
    }

    public override async Task Consumer(IConsumerContext<SendMessage> context)
    {
        var @event = new SendMessageChat(context.Message.Context);

        await _chatProcess.Include(@event).ConfigureAwait(false);
        context.NotifyConsumed();
    }
}
