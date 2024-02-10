using Sample.ChatHub.Bus;

namespace Sample.ChatHub.Domain.Contracts;

[Contract("create-chat")]
public record CreateChat(string Name, Guid IdUser);