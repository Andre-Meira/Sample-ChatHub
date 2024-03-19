using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Core.Chat.Events;
using Sample.ChatHub.Domain.Abstracts;
using Sample.ChatHub.Domain.Abstracts.EventStream;

namespace Sample.ChatHub.Worker.Core.Chat.Projections;

public sealed class ChatMembersProjection : ChatDecoratorProjection
{
    private Type[] EventsTypes = [typeof(ChatCreated), typeof(UserJoinedChat), typeof(UserLeftChat)];

    private readonly IRepositoreProjection<ChatMembers> _repositoreProjection;

    public ChatMembersProjection(IChatDecoratorProjection projection, 
        IRepositoreProjection<ChatMembers> repositoreProjection) : base(projection)
    {
        _repositoreProjection = repositoreProjection;
    }

    public override async Task Apply(IChatEventStream @event, CancellationToken cancellation = default)
    {
        bool isValid = @event.IsAny(EventsTypes);

        if (isValid == false)
        {
            await base.Apply(@event);
            return;
        }
        ChatMembers? chatMembers = await _repositoreProjection.GetAsync(@event.IdCorrelation, cancellation);

        chatMembers = ApplyEvent(@event, chatMembers);

        if (chatMembers is null)
            throw new ArgumentNullException(nameof(chatMembers));

        await _repositoreProjection.ProjectAsync(chatMembers, cancellation).ConfigureAwait(false);
        await base.Apply(@event);
    }

    private static ChatMembers? ApplyEvent(IChatEventStream @event, ChatMembers? chatMembers)
    {
        return @event switch
        {
            ChatCreated => ChatMembers.Apply((ChatCreated)@event),
            UserJoinedChat => chatMembers?.Apply((UserJoinedChat)@event),
            UserLeftChat => chatMembers?.Apply((UserLeftChat)@event),
            _ => null
        };
    }
}


public record ChatMembers : IAggregateProjection
{
    public ChatMembers(Guid idChat)
    {
        Id = idChat;
        Users = new List<Guid> { };
    }
    
    public List<Guid> Users { get; set; }
    public Guid Id { get; set; }

    public static ChatMembers Apply(ChatCreated @event)
    {
        var chat = new ChatMembers(@event.IdCorrelation);
        chat.Users.Add(@event.UserId);

        return chat;
    }

    public ChatMembers Apply(UserJoinedChat @event)
    {
        Users.Add(@event.UserId);
        return this;
    }

    public ChatMembers Apply(UserLeftChat @event)
    {
        Users.Remove(@event.UserId);
        return this;
    }
}
