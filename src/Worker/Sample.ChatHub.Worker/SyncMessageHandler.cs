using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Worker.Core.Messages;

namespace Sample.ChatHub.Worker;

internal class SyncMessageHandler : ConsumerHandlerBase<SyncUserMessage>
{
    private readonly ILogger<SyncMessageHandler> _logger;
    private readonly IMessageProcessStream _process;

    public SyncMessageHandler(IConnectionFactory connectionFactory,
        ILogger<SyncMessageHandler> logger,
        IMessageProcessStream process) : base(connectionFactory)
    {
        this.ExchageType = ExchangeType.Direct;
        this.PrefetchCount = 10;
        this.ExchangeName = "sync-message-consumer";

        _logger = logger;
        _process = process;
    }

    public override async Task Consumer(IConsumerContext<SyncUserMessage> context)
    {
        var a = await _process.GetMessagesToBeConfirmed(context.Message.UserId);
        //context.NotifyConsumed();        
    }
}