using  Sample.ChatHub.Worker.Core.Chat.Projections;

namespace Sample.ChatHub.Worker.Infrastructure.Models.Projections;

internal sealed record ChatMemberProjectionDB : ChatMembers
{
    public ChatMemberProjectionDB(ChatMembers original) : base(original)
    {
        Name = nameof(ChatMembers);
    }
    
    public string Name { get; init; } 
}
