using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Core.Chat.Events;
using Sample.ChatHub.Domain.Contracts;

namespace Sample.ChatHub.Worker.API.Consumers;

internal sealed class CreateChatHandlerConsumer : IConsumerHandler<CreateChat>
{
    private readonly ILogger<CreateChatHandlerConsumer> _logger;
    private readonly IChatProcessStream _chatProcess;

    public CreateChatHandlerConsumer(
        ILogger<CreateChatHandlerConsumer> logger, 
        IChatProcessStream chatProcess)
    {
        _logger = logger;
        _chatProcess = chatProcess;
    }

    public async Task Consumer(IConsumerContext<CreateChat> context)
    {
        _logger.LogInformation("Processando o chat {0}", context.Message.Name);

        var message = context.Message;
        var @event = new ChatCreated(message.IdChat, message.Name, message.IdUser);

        await _chatProcess.Include(@event).ConfigureAwait(false);        

        context.NotifyConsumed();
    }
}
