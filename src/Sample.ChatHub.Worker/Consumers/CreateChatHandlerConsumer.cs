using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Sample.ChatHub.Bus;
using Sample.ChatHub.Domain.Contracts;

namespace Sample.ChatHub.Worker;

internal sealed class CreateChatHandlerConsumer : ConsumerHandlerBase<CreateChat>
{
    private readonly ILogger<CreateChatHandlerConsumer> _logger;
    public CreateChatHandlerConsumer(IConnectionFactory connectionFactory, 
        ILogger<CreateChatHandlerConsumer> logger) : base(connectionFactory)
    {
        this.ExchageType = ExchangeType.Direct;
        this.PrefetchCount = 10;
        this.ExchangeName = "create-chat-consumer";

        _logger = logger;
    }

    public override async Task Consumer(IConsumerContext<CreateChat> context)
    {
        _logger.LogInformation("Processando o chat {0}", context.Message.Name);
        await Task.Delay(TimeSpan.FromSeconds(5));
        _logger.LogInformation("Canal processado.");

        context.NotifyConsumed();
    }
}
