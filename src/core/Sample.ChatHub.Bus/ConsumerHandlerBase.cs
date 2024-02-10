using System.Text;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Sample.ChatHub.Bus;

public abstract class ConsumerHandlerBase<TMessage> : BackgroundService, IDisposable
    where TMessage : class 
{
    #region Configuracao
    private readonly IModel _channel;

    protected string ExchangeName { get; init; } = string.Empty;
    protected string RoutingKey { get; init;} = string.Empty;
    protected string ExchageType { get; init; } = string.Empty;
    protected ushort PrefetchCount { get ; init;}    

    protected string MessageExchangeName => ContractExtensions.GetExchangeContract<TMessage>();
    protected string MessageExchangeType => ContractExtensions.GetExchangeTypeContract<TMessage>();

    public ConsumerHandlerBase(IConnectionFactory connectionFactory)
    {
        IConnection connection  = connectionFactory.CreateConnection();
        _channel = connection.CreateModel();                    
    }        

    private void Initilize()
    {
        _channel.ExchangeDeclare(ExchangeName, ExchageType, true);        
        _channel.ExchangeDeclare(MessageExchangeName, MessageExchangeType.ToString(), true);

        _channel.QueueDeclare(ExchangeName, true, false, false);
        _channel.QueueBind(ExchangeName, ExchangeName, RoutingKey, null);

        _channel.ExchangeBind(ExchangeName, MessageExchangeName, RoutingKey);

        if (PrefetchCount > 0) _channel.BasicQos(0, PrefetchCount, false);
        
    }
    #endregion
    
    public abstract Task Consumer(IConsumerContext<TMessage> context);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Initilize();

        await Task.Run(() => {
            var consumerEvent = new EventingBasicConsumer(_channel); 

            consumerEvent.Received += async (model, ea) =>
            {
                TMessage message = TransformMessage(ea);
                var context = new ConsumerContext<TMessage>(message, ea.DeliveryTag, _channel);
                await Consumer(context);
            };

            _channel.BasicConsume(ExchangeName, false, consumerEvent);
        });
    }

    private TMessage TransformMessage(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        return JsonConvert.DeserializeObject<TMessage>(message)!;
    }
}

