namespace Sample.ChatHub.Domain.Contracts.Messages;

public interface IMessageReceived
{
    public Guid IdMessage { get; init; }    
}
