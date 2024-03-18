namespace Sample.ChatHub.Bus;

public interface IConsumerHandler<T> : IConsumer where T : class
{
    public abstract Task Consumer(IConsumerContext<T> context);
}

public interface IConsumerFaultHandler<T> : IConsumerFault where T : class
{
    public abstract Task Consumer(IConsumerContext<T> context, Exception exception);
}

public interface IConsumer;

public interface IConsumerFault;