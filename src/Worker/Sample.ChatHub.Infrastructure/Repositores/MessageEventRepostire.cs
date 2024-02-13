using MongoDB.Bson;
using MongoDB.Driver;
using Sample.ChatHub.Domain.Contracts.Messages;
using Sample.ChatHub.Infrastructure.Context;
using Sample.ChatHub.Worker.Core.Messages;
using Sample.ChatHub.Worker.Core.Messages.Events;
using Sample.ChatHub.Worker.Infrastructure.Models;

namespace Sample.ChatHub.Infrastructure.Repositores;

internal class MessageEventsRepostiore : IMessageEventsRepositore
{
    private readonly MongoContext _context;
    public MessageEventsRepostiore(MongoContext context) => _context = context;

    public IEnumerable<IMessageEventStream> GetEvents(Guid idCorrelation)
    {       
        FilterDefinition<MessageEventStreamDB> filter = Builders<MessageEventStreamDB>.Filter
            .Eq(x => x.IdCorrelation, idCorrelation.ToString());

        List<IMessageEventStream> events = _context.Message.Find(filter)
            .ToList()
            .OrderBy(e => e.Event.DataProcessed)
            .Select(e => e.Event).ToList();

        return events;
    }

    public IEnumerable<Guid> GetMessagesToBeConfirmed(Guid IdUser)
    {
        var filter = Builders<MessageEventStreamDB>.Filter;

        var builderPrincipal = filter.Not(filter.Eq("Event.IdSender", IdUser.ToString())) 
                               & filter.Not(filter.Eq("Event.UserID", IdUser.ToString()))
                               & filter.Where(e => e.Event is SendMessageChat);

        var events = _context.Message.Find(builderPrincipal);
         
        return events.ToList()
            .OrderBy(e => e.Event.DataProcessed)
            .Select(e => e.Event.IdCorrelation);
    }

    public Task IncressEvent(IMessageEventStream @event) 
        => _context.Message.InsertOneAsync(new MessageEventStreamDB(@event));

}
