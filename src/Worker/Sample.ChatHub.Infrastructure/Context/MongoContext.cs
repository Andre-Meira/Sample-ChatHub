using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Sample.ChatHub.Core.Chat.Events;
using Sample.ChatHub.Infrastructure.Models;
using Sample.ChatHub.Domain.Abstracts.Options;
using Microsoft.Extensions.Options;
using Sample.ChatHub.Worker.Core.Messages.Events;
using Sample.ChatHub.Worker.Infrastructure.Models;

namespace Sample.ChatHub.Infrastructure.Context;

internal sealed class MongoContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<ChatEventStreamDB> Chat => _database.GetCollection<ChatEventStreamDB>("Chat");
    public IMongoCollection<MessageEventStreamDB> Message => _database.GetCollection<MessageEventStreamDB>("Message");

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
        BsonClassMap.RegisterClassMap<UserJoinedChat>();
        BsonClassMap.RegisterClassMap<UserLeftChat>();

        BsonClassMap.RegisterClassMap<SendMessageChat>(e =>
        {
            e.AutoMap();
            e.MapProperty(x => x.IdChat).SetSerializer(new GuidSerializer(BsonType.String));
            e.MapProperty(x => x.IdSender).SetSerializer(new GuidSerializer(BsonType.String));
        });        

        BsonClassMap.RegisterClassMap<ReceivedMessage>(e =>
        {
            e.AutoMap();

            e.MapProperty(x => x.UserID).SetSerializer(new GuidSerializer(BsonType.String));
            e.MapProperty(x => x.MessageId).SetSerializer(new GuidSerializer(BsonType.String));
            e.MapProperty(x => x.IdChat).SetSerializer(new GuidSerializer(BsonType.String));
        });
    }
}
