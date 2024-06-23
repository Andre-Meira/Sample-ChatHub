using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sample.ChatHub.Bus.Extesions;
using Sample.ChatHub.Bus.Models;
using Sample.ChatHub.Bus.Monitory;
using System.Text;

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
    private readonly IConnection _connection;


    protected string MessageExchangeName => MessageExtesions.GetExchangeContract<TMessage>();
    protected string MessageExchangeType => MessageExtesions.GetExchangeTypeContract<TMessage>();

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
        _connection = connection;

        Initilize();
        _logger = logger;
    }

    private void Initilize()
    {        
        if (IsRetryMessage)
        {            
            _channel.ExchangeDeclare(ExchageNameRetry, "topic", true, false);
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
        ActivityBus? activityBus = args.CreateConsumerActivityBus();
        activityBus?.Start();
        
        try
        {
            TMessage message = TransformMessage(args);
            var context = new ConsumerContext<TMessage>(message, args.DeliveryTag, _channel);
            await _consumerHandler.Consumer(context).ConfigureAwait(false);
        }
        catch (Exception err)
        {
            _logger.LogError("Consumer: {0}, error: {1}", _consumerHandler.GetType().Name, err.Message);
            activityBus?.AddExceptionEvent(err);
        }
        finally
        {
            activityBus?.Stop();
        }
    }

    private TMessage TransformMessage(BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);

            TMessage? message = JsonConvert.DeserializeObject<TMessage>(messageString);

            if (message is null)
            {
                throw new ArgumentException("message is null");
            }

            return message;
        }
        catch (Exception err)
        {
            _logger.LogError("Fail transform body the message, err: {0}", err.Message);
            _channel.BasicReject(eventArgs.DeliveryTag, false);
            throw;
        }
    }
    
    public override void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();        
    }
}

