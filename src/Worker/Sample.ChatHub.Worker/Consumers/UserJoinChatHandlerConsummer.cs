using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat.Events;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ChatHub.Worker.Consumers;

internal class UserJoinChatHandlerConsummer : ConsumerHandlerBase<UserJoinChat>
{
    private readonly IChatProcessStream _chatProcess;

    public UserJoinChatHandlerConsummer(
        IConnectionFactory connectionFactory,
        IChatProcessStream chatProcess)
    : base(connectionFactory)
    {
        this.ExchageType = ExchangeType.Direct;
        this.PrefetchCount = 20;
        this.ExchangeName = "user-join-consumer";
        _chatProcess = chatProcess;
    }

    public override async Task Consumer(IConsumerContext<UserJoinChat> context)
    {
        var @event = new UserJoinedChat(context.Message.ChatId, context.Message.UserId);

        await _chatProcess.Include(@event).ConfigureAwait(false);
        context.NotifyConsumed();
    }
}
