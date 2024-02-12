
namespace Sample.ChatHub.Worker.Core.Messages;

public interface IMessageEventsRepositore
{
    public IEnumerable<IMessageEventStream> GetEvents(Guid idCorrelation);

    public Task IncressEvent(IMessageEventStream @event);

    public IEnumerable<Guid> GetMessagesToBeConfirmed(Guid IdChat, Guid IdUser);
}
