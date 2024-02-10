using Sample.ChatHub.Domain.Abstracts.EventStream;

namespace Sample.ChatHub.Core.Chat;

public interface IChatEventStream : IEventStream
{
    public void Process(ChatHub chat);
}
