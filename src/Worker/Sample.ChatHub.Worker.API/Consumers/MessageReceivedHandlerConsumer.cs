using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Worker.Core.Messages.Events;

namespace Sample.ChatHub.Worker.API.Consumers;

internal class MessageReceivedHandlerConsumer : IConsumerHandler<MessageReceived>
{
    private readonly IMessageProcessStream _messageProcess;

    public MessageReceivedHandlerConsumer(IMessageProcessStream messageProcess)
    {
        _messageProcess = messageProcess;
    }

    public async Task Consumer(IConsumerContext<MessageReceived> context)
    {
        var @event = new ReceivedMessage(context.Message.IdChat, context.Message.IdMessage, 
            context.Message.IdUser);

        await _messageProcess.Include(@event).ConfigureAwait(false);
        context.NotifyConsumed();
    }
}
