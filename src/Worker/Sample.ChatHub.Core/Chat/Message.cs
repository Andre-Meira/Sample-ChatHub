namespace Sample.ChatHub.Core.Chat;

public record Message
{
    public Guid Id { get; init; }
    public byte[] Text { get; init; }
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }

    public Message(byte[] message, Guid userId)
    {
        Id = Guid.NewGuid();

        Date = DateTime.Now;    
        Text = message;
        UserId = userId;
    }
}
