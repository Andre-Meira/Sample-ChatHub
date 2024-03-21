using MongoDB.Driver;
using Sample.ChatHub.Domain.Abstracts;
using Sample.ChatHub.Domain.Abstracts.EventStream;
using Sample.ChatHub.Infrastructure.Context;
using Sample.ChatHub.Worker.Core.Chat.Projections;
using Sample.ChatHub.Worker.Infrastructure.Models.Projections;
using System.Linq.Expressions;

namespace Sample.ChatHub.Worker.Infrastructure.Repositores.Projections;

internal sealed class ChatMemberRepositoreProjection : IRepositoreProjection<ChatMembers>
{
    private readonly MongoContext _context;
    public ChatMemberRepositoreProjection(MongoContext context) => _context = context;

    public async Task<ChatMembers?> GetAsync(Guid IdProjection, CancellationToken cancellation = default)
    {
        FilterDefinitionBuilder<ChatMemberProjectionDB> filter = Builders<ChatMemberProjectionDB>.Filter;
        var builder = filter.Eq(e => e.Id, IdProjection);

        var chat = await _context.ChatMembers.FindAsync(builder, cancellationToken: cancellation);

        return chat.FirstOrDefault();
    }

    public async Task ProjectAsync(ChatMembers projection, CancellationToken cancellation = default)
    {
        FilterDefinitionBuilder<ChatMemberProjectionDB> filter = Builders<ChatMemberProjectionDB>.Filter;
        var builder = filter.Eq(e => e.Id, projection.Id);

        var chat = await _context.ChatMembers.FindAsync(builder, cancellationToken: cancellation);

        ChatMembers? chatMembers = chat.FirstOrDefault();

        if (chatMembers is null)
        {
            await _context.ChatMembers.InsertOneAsync(new ChatMemberProjectionDB(projection));
            return;
        }

        await _context.ChatMembers.ReplaceOneAsync(builder, new ChatMemberProjectionDB(projection)).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<ChatMembers> FindByFilterAsync(Expression<Func<ChatMembers, bool>> filter)
    {
        var expression = ExpressionHelper.ChangeParameter<ChatMembers, ChatMemberProjectionDB, bool>(filter);

        var events = await _context.ChatMembers.Find(expression).ToListAsync();

        foreach (var eventStream in events)
        {
            yield return eventStream;
        }
    }
}