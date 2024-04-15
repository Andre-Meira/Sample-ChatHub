using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Sample.ChatHub.Bus.Models;
using Sample.ChatHub.Bus.Resilience;

namespace Sample.ChatHub.Bus;

public static class BusConfiguration
{
    private static IConnection? _connection;

    public static IServiceCollection AddBus(this IServiceCollection services,
        IConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        services.AddScoped<IPublishContext, PublishContext>(e => new PublishContext(_connection));        

        return services;
    }

    public static IServiceCollection AddConsumer<TConsumerHandler, IMessage>(
        this IServiceCollection services, Action<IConsumerOptions> consumerOptions)
        where TConsumerHandler : IConsumer
        where IMessage : class
    {
        var consumer = GetConsumerInterface<TConsumerHandler>(typeof(IConsumerHandler<>));

        services.AddScoped(consumer, typeof(TConsumerHandler));

        IConsumerOptions optitons = new ConsumerOptions();
        consumerOptions.Invoke(optitons);

        if (optitons.FaultConfig is not null && optitons.FaultConfig.Consumer is not null)
        {
            var intarfaceFault = GetConsumerInterface<TConsumerHandler>(typeof(IConsumerFaultHandler<>));
            services.AddScoped(intarfaceFault, optitons.FaultConfig.Consumer);
        }

        ServiceProvider provider = services.BuildServiceProvider();

        IServiceScopeFactory providerFactory = provider.GetService<IServiceScopeFactory>()!;
        ILoggerFactory logFactory = provider.GetService<ILoggerFactory>()!;

        var log = logFactory.CreateLogger<ConsumerHandlerBase<IMessage>>();

        services.AddHostedService(e =>
        {
            var consumerHandlerInstance = Activator.CreateInstance(typeof(ConsumerHandlerBase<IMessage>),
                _connection, providerFactory, optitons, log);

            if (consumerHandlerInstance is null)
            {
                throw new ArgumentException("Não foi possivel criar a instancia");
            }

            return (ConsumerHandlerBase<IMessage>)consumerHandlerInstance;
        });

        return services;
    }

   
    private static Type GetConsumerInterface<TConsumerHandler>(Type baseType)
    {
        var consumer = typeof(TConsumerHandler).GetInterface(baseType.Name);

        if (consumer is null)
        {
            throw new ArgumentException($"O consumer {typeof(TConsumerHandler).Name}, não possui uma message vinculada");
        }

        return consumer;
    }
}
