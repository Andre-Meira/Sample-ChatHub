using Sample.ChatHub.Bus;

namespace Sample.ChatHub.Domain.Contracts.Messages;

[Contract("message-received")]
public record MessageReceived(Guid IdMessage, Guid IdUser);
