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
    }

    [BsonId]
    [DataMember]
    public MongoDB.Bson.ObjectId _id { get; set; }

    [DataMember]
    public MongoDB.Bson.BsonString? _t { get; set; } 

    public IChatEventStream Event { get; set; }

    public string IdCorrelation { get; init; }
}
