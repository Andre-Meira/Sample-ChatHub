namespace Sample.ChatHub.Domain.Abstracts.EventStream;

public interface IAggregateStream<EventStream> where EventStream : IEventStream
{
    void Apply(EventStream @event);
}
