using MongoDB.Bson.Serialization.Attributes;
using Sample.ChatHub.Worker.Core.Chat.Projections;
using System.Runtime.Serialization;

namespace Sample.ChatHub.Worker.Infrastructure.Models.Projections;

internal sealed class ChatMemberProjectionDB 
{
    public ChatMemberProjectionDB(ChatMembers chatMembers)
    {
        Id = chatMembers.Id.ToString();
        Data = chatMembers;
    }

    [BsonId]
    [DataMember]
    public MongoDB.Bson.ObjectId _id { get; set; }

    [DataMember]
    public MongoDB.Bson.BsonString? _t { get; set; }

    public string Id { get; init; }

    public ChatMembers Data { get; set; }
}
