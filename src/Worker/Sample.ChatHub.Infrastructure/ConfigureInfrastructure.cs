using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Infrastructure.Context;
using Sample.ChatHub.Infrastructure.Repositores;
using Sample.ChatHub.Worker.Core.Messages;

namespace Sample.ChatHub.Infrastructure;

public static class InfrastructureImplementation
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services)
    {
        var objectSerializer = new ObjectSerializer(x => true);
        BsonSerializer.RegisterSerializer(objectSerializer);

        MongoContextConfiguration.RegisterConfig();   
        services.AddTransient<MongoContext>();
        services.AddScoped<IChatEventsRepositore, ChatEventsRepostiore>();
        services.AddScoped<IMessageEventsRepositore, MessageEventsRepostiore>();

        return services;
    }
}
