namespace Sample.ChatHub.Domain.Contracts;

public interface IChatHub
{
    public Task SendMessage(Guid idChat, string message);    
    public Task ReceiveMessage(ReceiveMessageContext context);
}


public record ReceiveMessageContext(Guid IdChat, Guid IdUser, string Name,string Message);