using MongoDB.Driver;
using Sample.ChatHub.Domain.Abstracts.EventStream;
using Sample.ChatHub.Infrastructure.Context;
using Sample.ChatHub.Worker.Core.Chat.Projections;
using Sample.ChatHub.Worker.Infrastructure.Models.Projections;

namespace Sample.ChatHub.Worker.Infrastructure.Repositores.Projections;

internal sealed class ChatMemberRepositoreProjection : IRepositoreProjection<ChatMembers>
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

        var update = Builders<ChatMemberProjectionDB>.Update.Set(e => e.Data, projection);
        await _context.ChatMembers.UpdateOneAsync(builder, update).ConfigureAwait(false);
    }
}
