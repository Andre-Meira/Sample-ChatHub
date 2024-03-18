namespace Sample.ChatHub.Bus.Models;

public interface IFaultConsumerConfiguration
{
    public int Attempt { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public Type? Consumer { get; set; }
}

public class FaultConsumerConfiguration : IFaultConsumerConfiguration
{
    public FaultConsumerConfiguration(int attempt, TimeSpan timeSpan, Type? consumer = null)
    {
        Attempt = attempt;
        TimeSpan = timeSpan;
        Consumer = consumer;
    }

    public int Attempt { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public Type? Consumer { get; set; }
}
