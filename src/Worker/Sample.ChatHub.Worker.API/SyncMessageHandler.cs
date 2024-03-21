using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Worker.API.Services;
using Sample.ChatHub.Worker.Core.Messages;

namespace Sample.ChatHub.Worker.API;

internal class SyncMessageHandler : IConsumerHandler<SyncUserMessage>
{
    private readonly ILogger<SyncMessageHandler> _logger;
    private readonly IMessageProcessStream _process;
    private readonly SyncMessageService _syncService;

    public SyncMessageHandler(ILogger<SyncMessageHandler> logger,
        IMessageProcessStream process, SyncMessageService syncService)
    {
        _logger = logger;
        _process = process;
        _syncService = syncService;
    }

    public async Task Consumer(IConsumerContext<SyncUserMessage> context)
    {
        Guid userId = context.Message.UserId;
        IEnumerable<MessageHub> messages = await _process.GetMessagesToBeConfirmed(userId);

        var messageList = messages.Where(e => e.MessageId != Guid.Empty).ToList();

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