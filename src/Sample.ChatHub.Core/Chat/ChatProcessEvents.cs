using Sample.ChatHub.Domain.Abstracts.EventStream;

namespace Sample.ChatHub.Core.Chat;

public interface IChatProcessStream : IProcessorEventStream<ChatHub, IChatEventStream>;

public sealed class ChatProcessStream : IChatProcessStream
{    
    private readonly IChatEventsRepositore _chatEvents;

    public ChatProcessStream(IChatEventsRepositore chatEvents)
    {
        _chatEvents = chatEvents;
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
