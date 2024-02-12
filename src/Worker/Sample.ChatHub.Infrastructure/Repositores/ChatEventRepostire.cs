using MongoDB.Driver;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Infrastructure.Context;
using Sample.ChatHub.Infrastructure.Models;

namespace Sample.ChatHub.Infrastructure.Repositores;

internal class ChatEventsRepostiore : IChatEventsRepositore
{
    private readonly MongoContext _context;
    public ChatEventsRepostiore(MongoContext context) => _context = context;

    public IEnumerable<IChatEventStream> GetEvents(Guid idPayment)
    {       
        FilterDefinition<ChatEventStreamDB> filter = Builders<ChatEventStreamDB>.Filter
            .Eq(x => x.Event.IdCorrelation, idPayment);

        List<IChatEventStream> events = _context.Eventos.Find(filter)
            .ToList()
            .OrderBy(e => e.Event.DataProcessed)
            .Select(e => (IChatEventStream)e.Event).ToList();

        return events;
    }

    public Task IncressEvent(IChatEventStream @event) 
        => _context.Eventos.InsertOneAsync(new ChatEventStreamDB(@event));

}
