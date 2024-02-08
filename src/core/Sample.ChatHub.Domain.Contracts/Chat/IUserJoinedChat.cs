namespace Sample.ChatHub.Domain.Contracts.Chat;

internal interface IUserJoinedChat
{
    public Guid UserId { get; init; }
    public Guid ChatId { get; init; }    
}
