namespace Sample.ChatHub.Domain.Abstracts.EventStream;

public interface IProcessorEventStream<ProcessStream, EventStream>    
    where ProcessStream : IAggregateStream<EventStream>
    where EventStream : IEventStream
{
    Task Include(EventStream @event);

    Task<ProcessStream> Process(Guid Id);

    IEnumerable<EventStream> GetEvents(Guid Id);
}
