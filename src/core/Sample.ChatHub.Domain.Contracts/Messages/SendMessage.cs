namespace Sample.ChatHub.Domain.Contracts.Messages;

/// <summary>
/// Contrato Responsavel por mandar as messages do sistema.
/// </summary>
/// <param name="IdChat">Id do canal que sera feito o envio da message</param>
/// <param name="Sender">Id do remetente que envio a message</param>
/// <param name="message">Message que sera enviada</param>
public record SendMessage(Guid IdChat, Guid Sender, byte[] message) 
{
    public Guid IdMessage => Guid.NewGuid();
    public DateTime DateMessage => DateTime.UtcNow;
}

