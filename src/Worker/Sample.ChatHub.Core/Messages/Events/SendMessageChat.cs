using Sample.ChatHub.Domain.Contracts.Messages;

namespace Sample.ChatHub.Worker.Core.Messages.Events;

public class SendMessageChat : IMessageEventStream
{
    public SendMessageChat(ContextMessage context)
    {
        IdCorrelation = context.IdMessage;
        IdChat = context.IdChat;        

        UserId = context.IdSender;
        Message = context;
        DataProcessed = DateTime.Now;
    }

    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }
    public ContextMessage Message { get; init; }
    public Guid IdChat { get; init; }    
    public Guid UserId { get; init; }

    public void Process(MessageHub chat)
    {
        chat.MessageId = Message.IdMessage;
        chat.ChatId = IdCorrelation;
        chat.SenderId = Message.IdSender;

        chat.Timestamp = Message.Date;
        chat.Message = Message.Text;
    }
}
