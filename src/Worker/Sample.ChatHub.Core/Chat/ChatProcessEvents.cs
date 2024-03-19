using Sample.ChatHub.Domain.Abstracts.EventStream;
using Sample.ChatHub.Worker.Core.Chat.Projections;

namespace Sample.ChatHub.Core.Chat;

public interface IChatProcessStream : IProcessorEventStream<ChatHub, IChatEventStream>
{
    public Task<IEnumerable<Guid>> GetChatByUser(Guid userId, CancellationToken cancellationToken = default);
}

public sealed class ChatProcessStream : IChatProcessStream
{    
    private readonly IChatEventsRepositore _chatEvents;
    private readonly IChatDecoratorProjection _chatDecoratorProjection;
    private readonly IRepositoreProjection<ChatMembers> _repositoreProjection;

    public ChatProcessStream(IChatEventsRepositore chatEvents,
        IChatDecoratorProjection chatDecoratorProjection,
        IRepositoreProjection<ChatMembers> repositoreProjection)
    {
        _chatEvents = chatEvents;
        _chatDecoratorProjection = chatDecoratorProjection;
        _repositoreProjection = repositoreProjection;
    }

    public async Task<IEnumerable<Guid>> GetChatByUser(Guid userId, CancellationToken cancellationToken = default)
    {
        var chatsId = new List<Guid>();

        IAsyncEnumerable<ChatMembers> chats = _repositoreProjection.FindByFilterAsync(e => e.Users.Contains(userId));

        await foreach (ChatMembers chat in chats)
        {           
            if (chat.Users.Contains(userId)) chatsId.Add(chat.Id);                
        }

        return chatsId;
    }

    public IEnumerable<IChatEventStream> GetEvents(Guid Id)
    {
        return _chatEvents.GetEvents(Id);
    }

    public async Task Include(IChatEventStream @event)
    {
        ChatHub stream = await Process(@event.IdCorrelation);
        stream.Apply(@event);

        await _chatEvents.IncressEvent(@event).ConfigureAwait(false);        
        await _chatDecoratorProjection.Apply(@event).ConfigureAwait(false);
    }

    public Task<ChatHub> Process(Guid Id)
    {
        IEnumerable<IChatEventStream> events = GetEvents(Id);
        ChatHub paymentEvent = new ChatHub();

        foreach (IChatEventStream @event in events) paymentEvent.Apply(@event);

        return Task.FromResult(paymentEvent);
    }
}
