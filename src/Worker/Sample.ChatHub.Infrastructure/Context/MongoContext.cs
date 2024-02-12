using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Sample.ChatHub.Core.Chat.Events;
using Sample.ChatHub.Infrastructure.Models;
using Sample.ChatHub.Domain.Abstracts.Options;
using Microsoft.Extensions.Options;

namespace Sample.ChatHub.Infrastructure.Context;

internal sealed class MongoContext
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<ChatEventStreamDB> Eventos
        => _database.GetCollection<ChatEventStreamDB>("Chat");

    public MongoContext(IOptions<MongoOptions> options)
    {
        string connection = options.Value.Connection;
        string dataBaseName = options.Value.DatabaseName;

        var client = new MongoClient(connection);
        _database = client.GetDatabase(dataBaseName);       
    }
}

internal sealed class MongoContextConfiguration
{
    public static void RegisterConfig()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.CSharpLegacy));

        BsonClassMap.RegisterClassMap<ChatCreated>();
        BsonClassMap.RegisterClassMap<SendMessageChat>();        
        BsonClassMap.RegisterClassMap<UserJoinedChat>();
        BsonClassMap.RegisterClassMap<UserLeftChat>();
    }
}

