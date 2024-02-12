namespace Sample.ChatHub.Core.Chat;

public record Message
{
    public Guid Id { get; init; }
    public string Text { get; init; }
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }

    public Message(string message, Guid userId)
    {
        Id = Guid.NewGuid();

        Date = DateTime.Now;    
        Text = message;
        UserId = userId;
    }
}
