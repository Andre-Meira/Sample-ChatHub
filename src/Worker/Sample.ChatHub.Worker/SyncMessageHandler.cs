using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Worker.Core.Messages;

namespace Sample.ChatHub.Worker;

internal class SyncMessageHandler : ConsumerHandlerBase<SyncUserMessage>
{
    private readonly ILogger<CreateChatHandlerConsumer> _logger;
    private readonly IMessageEventsRepositore _respo;

    public SyncMessageHandler(IConnectionFactory connectionFactory,
        ILogger<CreateChatHandlerConsumer> logger,
        IMessageEventsRepositore repos) : base(connectionFactory)
    {
        this.ExchageType = ExchangeType.Direct;
        this.PrefetchCount = 10;
        this.ExchangeName = "sync-message-consumer";

        _logger = logger;
        _respo = repos;
    }

    public override Task Consumer(IConsumerContext<SyncUserMessage> context)
    {
        var guild = Guid.Parse("07718859-6209-464e-891c-c761035d9980");

        var a = _respo.GetMessagesToBeConfirmed(guild, context.Message.UserId);

        return Task.CompletedTask;
    }
}