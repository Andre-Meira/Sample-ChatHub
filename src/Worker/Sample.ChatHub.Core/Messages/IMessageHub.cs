using Sample.ChatHub.Domain.Abstracts.EventStream;

namespace Sample.ChatHub.Worker.Core.Messages;

public record MessageHub : IAggregateStream<IMessageEventStream>
{
    public Guid MessageId { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }

    public DateTime Timestamp { get; set; }
    public string Message { get; set; } = null!;

    private readonly List<Guid> _readByUserIds = new List<Guid>();
    public IReadOnlyList<Guid> ReadByUserIds => _readByUserIds;

    public void ReadMessage(Guid idUser) => _readByUserIds.Add(idUser);

    public void Apply(IMessageEventStream @event) => @event.Process(this);

}
