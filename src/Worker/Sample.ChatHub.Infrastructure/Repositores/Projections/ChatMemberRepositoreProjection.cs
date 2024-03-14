using MongoDB.Bson;
using MongoDB.Driver;
using Sample.ChatHub.Domain.Abstracts.EventStream;
using Sample.ChatHub.Infrastructure.Context;
using Sample.ChatHub.Infrastructure.Models;
using Sample.ChatHub.Worker.Core.Chat.Projections;
using Sample.ChatHub.Worker.Infrastructure.Models.Projections;

namespace Sample.ChatHub.Worker.Infrastructure.Repositores.Projections;

internal sealed class ChatMemberRepositoreProjection : IRepositoreProjection<ChatMembers, ChatMembersFilter>
{
    private readonly MongoContext _context;
    public ChatMemberRepositoreProjection(MongoContext context) => _context = context;

    public async Task<ChatMembers?> GetAsync(Guid IdProjection, CancellationToken cancellation = default)
    {
        FilterDefinitionBuilder<ChatMemberProjectionDB> filter = Builders<ChatMemberProjectionDB>.Filter;
        var builder = filter.Eq(e => e.Id, IdProjection.ToString());

        var chat = await _context.ChatMembers.FindAsync(builder, cancellationToken: cancellation);

        return chat.FirstOrDefault()?.Data;
    }

    public async Task ProjectAsync(ChatMembers projection, CancellationToken cancellation = default)
    {
        FilterDefinitionBuilder<ChatMemberProjectionDB> filter = Builders<ChatMemberProjectionDB>.Filter;
        var builder = filter.Eq(e => e.Id, projection.Id.ToString());

        var chat = await _context.ChatMembers.FindAsync(builder, cancellationToken: cancellation);

        ChatMembers? chatMembers = chat.FirstOrDefault()?.Data;

        if (chatMembers is null)
        {
            await _context.ChatMembers.InsertOneAsync(new ChatMemberProjectionDB(projection));
            return;
        }

        await _context.ChatMembers.ReplaceOneAsync(builder, new ChatMemberProjectionDB(projection)).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<ChatMembers> FindByFilterAsync(ChatMembersFilter filter)
    {                
        var events = _context.ChatMembers.Find(e => e.Data.Users.Contains(filter.IdUser));

        var a = await events.ToListAsync();

        foreach (var eventStream in a)
        {            
            yield return eventStream.Data;
        }
    }

    public async IAsyncEnumerable<Guid> GetChatsByUser(Guid userId)
    {
        FilterDefinitionBuilder<ChatEventStreamDB> filter = Builders<ChatEventStreamDB>.Filter;
        var builder = filter.Eq(e => e.UserId, userId.ToString());

        var events = await _context.Chat.FindAsync(builder);

        foreach (var eventStream in events.ToList())
        {
            yield return eventStream.Event.IdCorrelation;
        }
    }
}
