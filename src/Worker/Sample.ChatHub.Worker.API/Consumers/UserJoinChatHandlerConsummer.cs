using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat.Events;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Chat;

namespace Sample.ChatHub.Worker.API.Consumers;

internal class UserJoinChatHandlerConsummer : IConsumerHandler<UserJoinChat>
{
    private readonly IChatProcessStream _chatProcess;

    public UserJoinChatHandlerConsummer(IChatProcessStream chatProcess)
    {
        _chatProcess = chatProcess;
    }

    public async Task Consumer(IConsumerContext<UserJoinChat> context)
    {
        var @event = new UserJoinedChat(context.Message.ChatId, context.Message.UserId);

        await _chatProcess.Include(@event).ConfigureAwait(false);
        context.NotifyConsumed();
    }
}
