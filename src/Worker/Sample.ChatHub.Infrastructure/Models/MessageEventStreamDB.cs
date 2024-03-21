using MongoDB.Bson.Serialization.Attributes;
using Sample.ChatHub.Worker.Core.Messages;
using System.Runtime.Serialization;

namespace Sample.ChatHub.Worker.Infrastructure.Models;

public class MessageEventStreamDB
{
    public MessageEventStreamDB(IMessageEventStream @event)
    {
        Event = @event;

        IdChat = @event.IdChat.ToString();
        IdCorrelation = @event.IdCorrelation.ToString();
        UserId = @event.UserId.ToString();
    }

    [BsonId]
    [DataMember]
    public MongoDB.Bson.ObjectId _id { get; set; }

    [DataMember]
    public MongoDB.Bson.BsonString? _t { get; set; }

    public IMessageEventStream Event { get; set; }

    public string IdCorrelation { get; init; }

    public string UserId { get; init; }

    public string IdChat { get; init; }
}

