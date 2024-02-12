using Sample.ChatHub.Bus;

namespace Sample.ChatHub.Domain.Contracts.Messages;

/// <summary>
/// Contrato para sincronizar messages não recebidas pelo usuario
/// </summary>
/// <param name="user"></param>
[Contract("sync-message")]
public record SyncUserMessage(Guid UserId);
