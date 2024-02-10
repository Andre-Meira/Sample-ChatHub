namespace Sample.ChatHub.Server.API;


public class UserSettings
{
    public List<User>? Users { get; set; }

    public const string Key = "Users";
}

public class User
{
    public User(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
}