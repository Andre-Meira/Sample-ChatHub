using MongoDB.Bson.Serialization.Attributes;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Domain.Abstracts.EventStream;
using System.Runtime.Serialization;

namespace Sample.ChatHub.Infrastructure.Models;

internal class ChatEventStreamDB
{
    public ChatEventStreamDB(IChatEventStream @event)
    {
        Event = @event;
        IdCorrelation = @event.IdCorrelation.ToString();                 
        UserId = @event.UserId.ToString();
        EventName = nameof(EventName);
    }

    [BsonId]
    [DataMember]
    public MongoDB.Bson.ObjectId _id { get; set; }

    [DataMember]
    public MongoDB.Bson.BsonString? _t { get; set; } 

    public string EventName { get; init; }

    public IChatEventStream Event { get; set; }

    public string IdCorrelation { get; init; }

    public string UserId { get; init; }    
}
