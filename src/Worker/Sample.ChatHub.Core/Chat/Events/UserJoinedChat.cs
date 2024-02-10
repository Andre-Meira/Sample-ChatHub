namespace Sample.ChatHub.Core.Chat.Events;

public record class UserJoinedChat : IChatEventStream
{
    public UserJoinedChat(Guid chatId, Guid idUser)
    {
        IdUser = idUser;

        IdCorrelation = chatId;
        DataProcessed = DateTime.Now;
    }

    public Guid IdUser { get; init; }

    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }

    public void Process(ChatHub chat) => chat.AddUser(IdUser);    
}
