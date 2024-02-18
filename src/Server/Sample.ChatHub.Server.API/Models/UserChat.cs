namespace Sample.ChatHub.Server.API;

public record UserChats
{
    public UserChats(Guid userId)
    {
        UserId = userId;
        IdChats = new List<Guid>();
    }

    public Guid UserId { get; init; }
    public List<Guid> IdChats {get; set;}
}

