using Sample.ChatHub.Domain.Abstracts.EventStream;
using Sample.ChatHub.Worker.Core.Messages;

namespace Sample.ChatHub.Core.Chat;

public interface IMessageProcessStream : IProcessorEventStream<MessageHub, IMessageEventStream>
{
    Task<IEnumerable<MessageHub>> GetMessagesToBeConfirmed(Guid User);
}


public sealed class MessageProcessStream : IMessageProcessStream
{    
    private readonly IMessageEventsRepositore _messageEvents;
    private readonly IChatProcessStream _chatEvents;

    public MessageProcessStream(IMessageEventsRepositore messageEvents, IChatProcessStream chatEvents)
    {
        _messageEvents = messageEvents;
        _chatEvents = chatEvents;
    }

    public IEnumerable<IMessageEventStream> GetEvents(Guid Id)
    {
        return _messageEvents.GetEvents(Id);
    }

    public async Task<IEnumerable<MessageHub>> GetMessagesToBeConfirmed(Guid User)
    {
        IEnumerable<Guid> idMessages = _messageEvents.GetMessagesToBeConfirmed(User);

        List<MessageHub> messageHubs = new List<MessageHub>();

        await Parallel.ForEachAsync(idMessages, async (Guid id, CancellationToken cancellation) =>
        {
            var messageHub = await Process(id);
            messageHubs.Add(messageHub);
        });

        return messageHubs;
    }

    public async Task Include(IMessageEventStream @event)
    {
        ChatHub chat =  await _chatEvents.Process(@event.IdChat)
            .ConfigureAwait(false);

        if (chat.ChatId == Guid.Empty)
            throw new ArgumentException("O chat que essa message esta sendo enviado não existe.");

        MessageHub stream = await Process(@event.IdCorrelation);
        stream.Apply(@event);

        await _messageEvents.IncressEvent(@event).ConfigureAwait(false);
    }

    public Task<MessageHub> Process(Guid Id)
    {
        IEnumerable<IMessageEventStream> events = GetEvents(Id);
        MessageHub paymentEvent = new MessageHub();

        foreach (IMessageEventStream @event in events) paymentEvent.Apply(@event);

        return Task.FromResult(paymentEvent);
    }
}
