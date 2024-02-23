
using Sample.ChatHub.Worker.Core.Messages.Events;

namespace Sample.ChatHub.Worker.Core.Messages;

public interface IMessageEventsRepositore
{
    public IEnumerable<IMessageEventStream> GetEvents(Guid idCorrelation);
    
    public Task IncressEvent(IMessageEventStream @event);
    
    /// <summary>
    ///  Obtem id Messages que o usuario não confirmou a leietura
    /// </summary>
    /// <param name="IdUser"></param>
    /// <returns>
    ///     Retorna os ID Message
    /// </returns>
    public IEnumerable<Guid> GetMessagesToBeConfirmed(Guid IdUser);
}
