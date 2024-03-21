
namespace Sample.ChatHub.Core.Chat.Events;
public record UserLeftChat : IChatEventStream
{
    public UserLeftChat(Guid idChat, Guid idUser)
    {
        IdCorrelation = idChat;
        DataProcessed = DateTime.Now;

        UserId = idUser;
    }

    public Guid UserId { get; init; }
    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }

    public void Process(ChatHub chat) => chat.RemoveUser(UserId);
}
