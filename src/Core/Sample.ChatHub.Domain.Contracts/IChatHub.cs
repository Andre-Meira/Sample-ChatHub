using Sample.ChatHub.Domain.Contracts.Messages;

namespace Sample.ChatHub.Domain.Contracts;

public interface IChatHub
{
    public Task SendMessage(Guid idChat, string message);    
    public Task ReceiveMessage(ContextMessage context);
}
