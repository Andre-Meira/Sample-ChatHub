namespace Sample.ChatHub.Domain.Contracts.Chat;

public interface IChatCreated
{
    Guid Id { get; init; }

    string Name { get; init; }   
}
