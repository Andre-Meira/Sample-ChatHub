namespace Sample.ChatHub.Server.API;

public record UserChats
{
    public UserChats(Guid userId)
    {
        UserId = userId;
        IdChats = new List<string>();
    }

    public Guid UserId { get; init; }
    public List<string> IdChats { get; set; }
}

