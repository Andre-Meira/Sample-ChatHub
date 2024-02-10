namespace Sample.ChatHub.Core.Chat.Events;

public record ChatCreated : IChatEventStream
{
    public ChatCreated(string name, Guid idUser)
    {
        Name = name;
        IdUser = idUser;

        DataProcessed = DateTime.Now;
        IdCorrelation = Guid.NewGuid(); 
    }
    
    public string Name { get; init; }
    public Guid IdUser { get; init; }

    public Guid IdCorrelation { get; init; }
    public DateTime DataProcessed { get; init; }

    public void Process(ChatHub chat)
    {
        chat.Create(IdCorrelation);
        chat.AddUser(IdUser);        
        
        chat.UserCreated = IdUser;
        chat.Name = Name;        
    }
}
