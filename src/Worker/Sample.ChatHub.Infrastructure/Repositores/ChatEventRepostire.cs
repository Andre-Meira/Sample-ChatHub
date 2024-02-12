using MongoDB.Driver;
using Sample.ChatHub.Core.Chat;
using Sample.ChatHub.Infrastructure.Context;
using Sample.ChatHub.Infrastructure.Models;

namespace Sample.ChatHub.Infrastructure.Repositores;

internal class ChatEventsRepostiore : IChatEventsRepositore
{
    private readonly MongoContext _context;
    public ChatEventsRepostiore(MongoContext context) => _context = context;

    public IEnumerable<IChatEventStream> GetEvents(Guid idCorrelation)
    {       
        FilterDefinition<ChatEventStreamDB> filter = Builders<ChatEventStreamDB>.Filter
            .Eq(x => x.IdCorrelation, idCorrelation.ToString());

        List<IChatEventStream> events = _context.Chat.Find(filter)
            .ToList()
            .OrderBy(e => e.Event.DataProcessed)
            .Select(e => e.Event).ToList();

        return events;
    }

    public Task IncressEvent(IChatEventStream @event) 
        => _context.Chat.InsertOneAsync(new ChatEventStreamDB(@event));

}
