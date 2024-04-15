using Sample.ChatHub.Bus;

namespace Sample.ChatHub.Domain.Contracts.Chat;

[Message("user-join-chat")]
public record UserJoinChat(Guid ChatId, Guid UserId);
