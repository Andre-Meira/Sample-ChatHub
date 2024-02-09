namespace Sample.ChatHub.Domain.Abstracts.Options;

public class BusOptions
{
    public const string Key = "Bus";

    public string Host { get; set; } = null!;
    public string VirtualHost { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public BusOptions() { }
}
