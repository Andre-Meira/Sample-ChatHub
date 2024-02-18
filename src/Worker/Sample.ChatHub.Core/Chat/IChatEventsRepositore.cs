
namespace Sample.ChatHub.Core.Chat;

public interface IChatEventsRepositore
{
    public IEnumerable<IChatEventStream> GetEvents(Guid idChat);

    public Task IncressEvent(IChatEventStream @event);

    public Task<IEnumerable<Guid>> GetUserChats(Guid userId);
}
