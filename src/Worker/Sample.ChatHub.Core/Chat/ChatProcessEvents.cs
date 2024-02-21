using Sample.ChatHub.Domain.Abstracts.EventStream;

namespace Sample.ChatHub.Core.Chat;

public interface IChatProcessStream : IProcessorEventStream<ChatHub, IChatEventStream>
{
    public Task<IEnumerable<Guid>> GetChatByUser(Guid userId);
}

public sealed class ChatProcessStream : IChatProcessStream
{    
    private readonly IChatEventsRepositore _chatEvents;

    public ChatProcessStream(IChatEventsRepositore chatEvents)
    {
        _chatEvents = chatEvents;
    }

    public async Task<IEnumerable<Guid>> GetChatByUser(Guid userId)
    {
        var chats = new List<Guid>();
        IAsyncEnumerable<Guid> idChats = _chatEvents.GetChatsByUser(userId);

        await foreach (Guid idchat in idChats)
        {
            ChatHub chat =  await Process(idchat);

            if (chat.Users.Contains(userId)) chats.Add(idchat);                
        }

        return chats;
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
    }

    public Task<ChatHub> Process(Guid Id)
    {
        IEnumerable<IChatEventStream> events = GetEvents(Id);
        ChatHub paymentEvent = new ChatHub();

        foreach (IChatEventStream @event in events) paymentEvent.Apply(@event);

        return Task.FromResult(paymentEvent);
    }
}
