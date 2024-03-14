using MongoDB.Bson;
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
        Name = nameof(ChatMembers);
    }

    [BsonId]
    [DataMember]
    public string Id { get; init; }

    public ChatMembers Data { get; set; }
    public string Name { get; init; }
}
