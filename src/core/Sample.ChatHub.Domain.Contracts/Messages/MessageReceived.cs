using Sample.ChatHub.Bus;

namespace Sample.ChatHub.Domain.Contracts.Messages;

[Contract("message-received")]
public record MessageReceived(Guid IdChat, Guid IdMessage, Guid IdUser);
