using Sample.ChatHub.Domain.Abstracts.EventStream;

namespace Sample.ChatHub.Worker.Core.Messages;

public interface IMessageEventStream : IEventStream
{
    public Guid IdChat { get; init; }

    public void Process(MessageHub chat);
}
