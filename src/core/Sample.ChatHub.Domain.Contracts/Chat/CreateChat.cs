using Sample.ChatHub.Bus;

namespace Sample.ChatHub.Domain.Contracts;

[Contract("create-chat")]
public record CreateChat(Guid IdChat, string Name, Guid IdUser);