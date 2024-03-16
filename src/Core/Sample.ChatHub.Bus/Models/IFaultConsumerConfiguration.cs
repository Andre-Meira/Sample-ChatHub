namespace Sample.ChatHub.Bus.Models;

public interface IFaultConsumerConfiguration
{
    public int Attempt { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public Type Consumer { get; set; }
}
