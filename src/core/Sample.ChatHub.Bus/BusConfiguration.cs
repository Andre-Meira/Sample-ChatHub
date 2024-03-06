using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Runtime.InteropServices.ObjectiveC;

namespace Sample.ChatHub.Bus;

public static class BusConfiguration
{
    public static IServiceCollection AddBus(this IServiceCollection services, 
        IConnectionFactory connectionFactory)
    {
        services.AddScoped(e => connectionFactory);
        services.AddScoped<IPublishContext,PublishContext>();

        return services;
    }

    public static IServiceCollection AddConsumer<TConsumerHandler>(
        this IServiceCollection services, 
        Func<IConsumerOptions> consumerOptions) 
    where TConsumerHandler : IConsumer
    {
        var message = GetMessageConsumer<TConsumerHandler>();
        var consumer = GetConsumerInterface<TConsumerHandler>();

        services.AddScoped(consumer, typeof(TConsumerHandler));

        IConsumerOptions optitons = consumerOptions.Invoke();
        ServiceProvider provider = services.BuildServiceProvider();

        IServiceScopeFactory providerFactory = provider.GetService<IServiceScopeFactory>()!;
        IConnectionFactory connection = provider.GetService<IConnectionFactory>()!;

        services.AddHostedService(e =>
        {
            var consumerHandlerInstance = Activator.CreateInstance(typeof(ConsumerHandlerBase<>).MakeGenericType(message), 
                connection,providerFactory,optitons);
                       

            if (consumerHandlerInstance is null)
            {
                throw new ArgumentException("Não foi possivel criar a instancia");
            }

            return (BackgroundService)consumerHandlerInstance;
        });

        return services;
    }

    private static Type GetMessageConsumer<TConsumerHandler>() where TConsumerHandler : IConsumer
    {
        return GetConsumerInterface<TConsumerHandler>().GetGenericArguments().FirstOrDefault()!;            
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
