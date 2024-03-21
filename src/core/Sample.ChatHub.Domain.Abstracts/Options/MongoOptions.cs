namespace Sample.ChatHub.Domain.Abstracts.Options;

public class MongoOptions
{
    public const string Key = "Mongo";

    public string Connection { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public MongoOptions() { }
}
