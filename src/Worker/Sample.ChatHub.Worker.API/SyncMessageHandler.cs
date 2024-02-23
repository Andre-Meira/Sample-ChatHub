using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Worker.API.Services;
using Sample.ChatHub.Worker.Core.Messages;

namespace Sample.ChatHub.Worker.API;

internal class SyncMessageHandler : ConsumerHandlerBase<SyncUserMessage>
{
    private readonly ILogger<SyncMessageHandler> _logger;
    private readonly IMessageProcessStream _process;
    private readonly SyncMessageService _syncService;

    public SyncMessageHandler(IConnectionFactory connectionFactory,
        ILogger<SyncMessageHandler> logger,
        IMessageProcessStream process,
        SyncMessageService syncMessage) : base(connectionFactory)
    {
        this.ExchageType = ExchangeType.Direct;
        this.PrefetchCount = 10;
        this.ExchangeName = "sync-message-consumer";

        _logger = logger;
        _process = process;
        _syncService = syncMessage;
    }

    public override async Task Consumer(IConsumerContext<SyncUserMessage> context)
    {
        Guid userId = context.Message.UserId;
        IEnumerable<MessageHub> messages = await _process.GetMessagesToBeConfirmed(userId);
        
        var messageList = messages.ToList();

        if (messages.Any() == false) return;

        bool result = await _syncService.SyncMessagen(userId, messageList)
                .ConfigureAwait(false);

        if (result == false)
        {
            _logger.LogWarning("User not sync {0}.", userId);            
        }
        
        context.NotifyConsumed();
    }
}