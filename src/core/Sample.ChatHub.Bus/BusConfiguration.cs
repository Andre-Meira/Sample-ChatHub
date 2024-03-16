using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Runtime.InteropServices.ObjectiveC;

namespace Sample.ChatHub.Bus;

public static class BusConfiguration
{
    private static IConnection? _connection;

    public static IServiceCollection AddBus(this IServiceCollection services, 
        IConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        services.AddScoped<IPublishContext,PublishContext>(e => new PublishContext(_connection));

        return services;
    }

    public static IServiceCollection AddConsumer<TConsumerHandler, IMessage>(
        this IServiceCollection services, Action<IConsumerOptions> consumerOptions) 
        where TConsumerHandler : IConsumer 
        where IMessage : class
    {        
        var consumer = GetConsumerInterface<TConsumerHandler>();

        services.AddScoped(consumer, typeof(TConsumerHandler));

        IConsumerOptions optitons = new ConsumerOptions();
        consumerOptions.Invoke(optitons);

        ServiceProvider provider = services.BuildServiceProvider();

        IServiceScopeFactory providerFactory = provider.GetService<IServiceScopeFactory>()!;        

        services.AddHostedService(e =>
        {
            var consumerHandlerInstance = Activator.CreateInstance(typeof(ConsumerHandlerBase<IMessage>),
                _connection, providerFactory, optitons);                       

            if (consumerHandlerInstance is null)
            {
                throw new ArgumentException("Não foi possivel criar a instancia");
            }

            return (ConsumerHandlerBase<IMessage>)consumerHandlerInstance;
        });

        return services;
    }


    private static Type GetConsumerInterface<TConsumerHandler>()
    {
        Type baseType = typeof(IConsumerHandler<>);

        var consumer = typeof(TConsumerHandler).GetInterface(baseType.Name);

        if (consumer is null)
        {
            throw new ArgumentException($"O consumer {typeof(TConsumerHandler).Name}, não possui uma message vinculada");
        }

        return consumer;
    }
}
