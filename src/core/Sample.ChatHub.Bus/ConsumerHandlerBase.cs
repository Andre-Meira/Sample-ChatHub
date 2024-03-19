using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.ChatHub.Bus.Models;

namespace Sample.ChatHub.Bus;

public class ConsumerHandlerBase<TMessage> : BackgroundService, IDisposable
    where TMessage : class 
{
    #region Configuracao
    private readonly IModel _channel;
    
    private readonly IConsumerOptions _consumerOptions;
    private readonly IConsumerHandler<TMessage> _consumerHandler;
    private readonly IConsumerFaultHandler<TMessage>? _consumerFaultHandler;

    private readonly ILogger<ConsumerHandlerBase<TMessage>> _logger;    
    private readonly IFaultConsumerConfiguration? _configDeadLetter;    

    protected string MessageExchangeName => ContractExtensions.GetExchangeContract<TMessage>();
    protected string MessageExchangeType => ContractExtensions.GetExchangeTypeContract<TMessage>();

    private bool IsRetryMessage => _configDeadLetter is not null;
    private string ExchageNameRetry => _consumerOptions.ExchangeName + "-retry";

    public ConsumerHandlerBase(IConnection connection,
        IServiceScopeFactory serviceScopeFactory,
        IConsumerOptions consumerOptions,
        ILogger<ConsumerHandlerBase<TMessage>> logger)
    {
        using var service = serviceScopeFactory.CreateScope();
            
        if (consumerOptions.FaultConfig is not null)
        {            
            _consumerFaultHandler = service.ServiceProvider.GetRequiredService<IConsumerFaultHandler<TMessage>>();
            _configDeadLetter = consumerOptions.FaultConfig;            
        }

        _consumerHandler = service.ServiceProvider.GetRequiredService<IConsumerHandler<TMessage>>();
       
        _channel = connection.CreateModel();
        _consumerOptions = consumerOptions;

        Initilize();            
        _logger = logger;
    }

    private void Initilize()
    {
        var args = new Dictionary<string, object> { };

        if (IsRetryMessage)
        {           
            args.Add("x-delayed-type", _consumerOptions.ExchageType);

            _channel.ExchangeDeclare(ExchageNameRetry, "x-delayed-message", true, false, args);
            _channel.QueueDeclare(ExchageNameRetry, true, false, false);
            _channel.QueueBind(ExchageNameRetry, ExchageNameRetry, "", null);
        }

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
        var consumerEvent = new EventingBasicConsumer(_channel);

        await Task.Run(() =>
        {
            consumerEvent.Received += async (model, ea) => await ReceivedMessageAsync(ea);
            _channel.BasicConsume(_consumerOptions.ExchangeName, false, consumerEvent);

            if (IsRetryMessage)
            {
                _channel.BasicConsume(ExchageNameRetry, false, consumerEvent);
            }
        });        
    }

    private async Task ReceivedMessageAsync(BasicDeliverEventArgs args) 
    {
        TMessage message = TransformMessage(args);

        try
        {
            var context = new ConsumerContext<TMessage>(message, args.DeliveryTag, _channel);
            await _consumerHandler.Consumer(context).ConfigureAwait(false);
        }
        catch (Exception err)
        {
            _logger.LogError("Consumer: {0}, error: {1}", _consumerHandler.GetType().Name, err.Message);
            await RetryMessageAsync(message, args, err);
        }
    }
        
    private TMessage TransformMessage(BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        return JsonConvert.DeserializeObject<TMessage>(message)!;
    }

    private async Task RetryMessageAsync(TMessage message, BasicDeliverEventArgs basicDeliver, Exception exception)
    {
        if (IsRetryMessage == false) return;

        IBasicProperties properties = basicDeliver.BasicProperties ?? _channel.CreateBasicProperties();

        if (properties.IsHeadersPresent() == false)
        {
            properties.Headers = new Dictionary<string, object> 
            { { "x-delay", _configDeadLetter!.TimeSpan.TotalMilliseconds } };
        }

        properties.Headers["x-delay"] = _configDeadLetter!.TimeSpan.TotalMilliseconds;

        object? countCurrent;
        properties.Headers.TryGetValue("count", out countCurrent);
        int count = countCurrent == null ? 1 : (int)countCurrent + 1;
        properties.Headers["count"] = count;
        
        if (count > _configDeadLetter!.Attempt)
        {
            if (_consumerFaultHandler is not null)
            {
                var context = new ConsumerContext<TMessage>(message, basicDeliver.DeliveryTag, _channel);
                await _consumerFaultHandler.Consumer(context, exception);                                
            }

            _channel.BasicNack(basicDeliver.DeliveryTag, false, false);
            _logger.LogWarning("Message foi apagada.");

            return;
        }
        
        string json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(ExchageNameRetry, "", properties, body);
        _channel.BasicAck(basicDeliver.DeliveryTag, false);
    }
}

