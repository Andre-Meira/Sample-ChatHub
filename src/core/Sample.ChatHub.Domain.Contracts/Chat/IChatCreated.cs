namespace Sample.ChatHub.Domain.Contracts.Chat;

public interface IChatCreated
{
    Guid Id { get; set; }

    string Name { get; set; }   
}
