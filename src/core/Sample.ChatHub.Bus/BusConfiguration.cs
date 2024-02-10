using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

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
}
