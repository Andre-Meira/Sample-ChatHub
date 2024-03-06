using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Sample.ChatHub.Bus;

public class ConsumerHandlerBase<TMessage> : BackgroundService, IDisposable
    where TMessage : class 
{
    #region Configuracao
    private readonly IModel _channel;
    private readonly IConsumerOptions _consumerOptions;
    private readonly IConsumerHandler<TMessage> _consumerHandler;

    protected string MessageExchangeName => ContractExtensions.GetExchangeContract<TMessage>();
    protected string MessageExchangeType => ContractExtensions.GetExchangeTypeContract<TMessage>();

    public ConsumerHandlerBase(IConnectionFactory connectionFactory, 
        IServiceScopeFactory serviceScopeFactory, IConsumerOptions consumerOptions)
    {
        IConnection connection  = connectionFactory.CreateConnection();

        using var service = serviceScopeFactory.CreateScope();

        _consumerHandler = service.ServiceProvider.GetRequiredService<IConsumerHandler<TMessage>>();
        _channel = connection.CreateModel();                    
        _consumerOptions = consumerOptions;
    }        

    private void Initilize()
    {
        _channel.ExchangeDeclare(_consumerOptions.ExchangeName, _consumerOptions.ExchageType, true);        
        _channel.ExchangeDeclare(MessageExchangeName, MessageExchangeType.ToString(), true);

        _channel.QueueDeclare(_consumerOptions.ExchangeName, true, false, false);
        _channel.QueueBind(_consumerOptions.ExchangeName, _consumerOptions.ExchangeName, _consumerOptions.RoutingKey, null);

        _channel.ExchangeBind(_consumerOptions.ExchangeName, MessageExchangeName, _consumerOptions.RoutingKey);

        if (_consumerOptions.PrefetchCount > 0) _channel.BasicQos(0, _consumerOptions.PrefetchCount, false);
        
    }
    #endregion        

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Initilize();

        await Task.Run(() => {
            var consumerEvent = new EventingBasicConsumer(_channel); 

            consumerEvent.Received += async (model, ea) =>
            {
                TMessage message = TransformMessage(ea);
                var context = new ConsumerContext<TMessage>(message, ea.DeliveryTag, _channel);
                await _consumerHandler.Consumer(context).ConfigureAwait(false);
            };

            _channel.BasicConsume(_consumerOptions.ExchangeName, false, consumerEvent);
        });
    }

    private TMessage TransformMessage(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        return JsonConvert.DeserializeObject<TMessage>(message)!;
    }
}

