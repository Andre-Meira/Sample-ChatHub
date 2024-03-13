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
using Sample.ChatHub.Worker.Infrastructure.Models.Projections;
using Sample.ChatHub.Worker.Core.Chat.Projections;

namespace Sample.ChatHub.Infrastructure.Context;

internal sealed class MongoContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<ChatEventStreamDB> Chat => _database.GetCollection<ChatEventStreamDB>("Chat");
    public IMongoCollection<MessageEventStreamDB> Message => _database.GetCollection<MessageEventStreamDB>("Message");
    public IMongoCollection<ChatMemberProjectionDB> ChatMembers => _database.GetCollection<ChatMemberProjectionDB>("Projections");

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
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Local));

        BsonClassMap.RegisterClassMap<ChatCreated>();                
        BsonClassMap.RegisterClassMap<UserJoinedChat>();
        BsonClassMap.RegisterClassMap<UserLeftChat>();
        BsonClassMap.RegisterClassMap<ChatMembers>();

        BsonClassMap.RegisterClassMap<ChatMemberProjectionDB>();

        BsonClassMap.RegisterClassMap<SendMessageChat>();        

        BsonClassMap.RegisterClassMap<ReceivedMessage>();
        

        //BsonClassMap.RegisterClassMap<ChatCreated>();
        //BsonClassMap.RegisterClassMap<UserJoinedChat>();
        //BsonClassMap.RegisterClassMap<UserLeftChat>();

        //BsonClassMap.RegisterClassMap<ChatMembers>(e =>
        //{
        //    e.AutoMap();
        //    e.MapProperty(x => x.Users).SetSerializer(new EnumerableInterfaceImplementerSerializer<List<Guid>, Guid>(new GuidSerializer(GuidRepresentation.Standard)));
        //    e.MapProperty(x => x.Id).SetSerializer(new GuidSerializer(BsonType.String));
        //});

        //BsonClassMap.RegisterClassMap<ChatMemberProjectionDB>();

        //BsonClassMap.RegisterClassMap<SendMessageChat>(e =>
        //{
        //    e.AutoMap();
        //    e.MapProperty(x => x.IdChat).SetSerializer(new GuidSerializer(BsonType.String));
        //    e.MapProperty(x => x.UserId).SetSerializer(new GuidSerializer(BsonType.String));
        //});

        //BsonClassMap.RegisterClassMap<ReceivedMessage>(e =>
        //{
        //    e.AutoMap();

        //    e.MapProperty(x => x.UserId).SetSerializer(new GuidSerializer(BsonType.String));
        //    e.MapProperty(x => x.MessageId).SetSerializer(new GuidSerializer(BsonType.String));
        //    e.MapProperty(x => x.IdChat).SetSerializer(new GuidSerializer(BsonType.String));
        //});
    }
}
