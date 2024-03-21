using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Abstracts.EventStream;
using Sample.ChatHub.Infrastructure.Context;
using Sample.ChatHub.Infrastructure.Repositores;
using Sample.ChatHub.Worker.Core.Chat.Projections;
using Sample.ChatHub.Worker.Core.Messages;
using Sample.ChatHub.Worker.Infrastructure.Repositores.Projections;

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
        services.AddScoped<IRepositoreProjection<ChatMembers>, ChatMemberRepositoreProjection>();

        return services;
    }
}
