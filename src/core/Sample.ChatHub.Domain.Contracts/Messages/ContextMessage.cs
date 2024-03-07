namespace Sample.ChatHub.Domain.Contracts.Messages;

public record ContextMessage
{
    public ContextMessage(Guid idChat,Guid idSender, string userName, string text, DateTime date = default)
    {
        Text = text;
        IdSender = idSender;
        IdChat = idChat;

        IdMessage = Guid.NewGuid(); 
        Date = date == default ? DateTime.Now : date;

        UserName = userName;
    }

    public Guid IdMessage { get; init; } 
    
    public Guid IdChat { get; init; }       
    public Guid IdSender { get; init; }

    public DateTime Date { get; init; }

    public string Text { get; init; }

    public string UserName { get; set; }
}
