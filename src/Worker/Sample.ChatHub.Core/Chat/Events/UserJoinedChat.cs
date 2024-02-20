namespace Sample.ChatHub.Core.Chat.Events;

public record class UserJoinedChat : IChatEventStream
{
    public UserJoinedChat(Guid chatId, Guid idUser)
    {
        UserId = idUser;

        IdCorrelation = chatId;
        DataProcessed = DateTime.Now;
    }    

    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }
    public Guid UserId { get; init; }

    public void Process(ChatHub chat) => chat.AddUser(UserId);    
}
