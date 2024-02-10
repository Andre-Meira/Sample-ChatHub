
namespace Sample.ChatHub.Core.Chat.Events;

public class SendMessageChat : IChatEventStream
{
    public SendMessageChat(Guid chatId,Guid userId, byte[] message)
    {
        IdCorrelation = chatId;
        Message = new Message(message, userId);
        DataProcessed = DateTime.Now;

    }

    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }

    public Message Message { get; init; }
    public Guid IdUser { get; init; }

    public void Process(ChatHub chat) => chat.SendMessage(Message);
}
