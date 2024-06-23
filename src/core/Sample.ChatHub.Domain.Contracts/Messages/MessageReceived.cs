using Sample.ChatHub.Bus;

namespace Sample.ChatHub.Domain.Contracts.Messages;

[Message("message-received")]
public record MessageReceived(Guid IdChat, Guid IdMessage, Guid IdUser);
