using Sample.ChatHub.Core.Chat;

namespace Sample.ChatHub.Worker.Core.Messages.Events;

public class ReceivedMessage : IMessageEventStream
{
    public ReceivedMessage(Guid idChat, Guid messageId, Guid userID)
    {        
        MessageId = messageId;
        UserID = userID;

        IdChat = IdChat;
        IdCorrelation = messageId;
        DataProcessed = DateTime.Now;
    }
    
    public Guid UserID { get; init; }
    public Guid MessageId { get; init; }

    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }
    public Guid IdChat { get; init; }

    public void Process(MessageHub chat) => chat.ReadMessage(UserID);    
}
