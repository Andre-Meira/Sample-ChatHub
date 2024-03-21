namespace Sample.ChatHub.Core.Chat.Events;

public record ChatCreated : IChatEventStream
{
    public ChatCreated(Guid idChat, string name, Guid idUser)
    {
        Name = name;

        UserId = idUser;
        DataProcessed = DateTime.Now;
        IdCorrelation = idChat;
    }

    public string Name { get; init; }

    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }
    public Guid UserId { get; init; }

    public void Process(ChatHub chat)
    {
        chat.Create(IdCorrelation);
        chat.AddUser(UserId);

        chat.UserCreated = UserId;
        chat.Name = Name;
    }
}
