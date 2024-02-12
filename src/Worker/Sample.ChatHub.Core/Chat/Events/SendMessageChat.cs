
using Sample.ChatHub.Domain.Contracts.Messages;

namespace Sample.ChatHub.Core.Chat.Events;

public class SendMessageChat : IChatEventStream
{
    public SendMessageChat(ContextMessage context)
    {
        IdCorrelation = context.IdChat;
        Message = context;
        DataProcessed = DateTime.Now;

    }

    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }

    public ContextMessage Message { get; init; }    

    public void Process(ChatHub chat) => chat.SendMessage(Message);
}
