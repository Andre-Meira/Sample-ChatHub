using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Worker.Core.Messages.Events;

namespace Sample.ChatHub.Worker.API.Consumers;

internal sealed class SendMessageHandlerConsumer : IConsumerHandler<SendMessage>
{
    private readonly IMessageProcessStream _messageProcess;

    public SendMessageHandlerConsumer(IMessageProcessStream messageProcess)
    {
        _messageProcess = messageProcess;
    }

    public async Task Consumer(IConsumerContext<SendMessage> context)
    {
        var @event = new SendMessageChat(context.Message.Context);

        await _messageProcess.Include(@event).ConfigureAwait(false);
        context.NotifyConsumed();
    }
}
