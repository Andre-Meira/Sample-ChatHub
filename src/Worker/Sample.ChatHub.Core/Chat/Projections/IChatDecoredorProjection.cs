using Sample.ChatHub.Core.Chat;

namespace Sample.ChatHub.Worker.Core.Chat.Projections;

public interface IChatDecoratorProjection
{
    Task Apply(IChatEventStream @event, CancellationToken cancellation = default);
}

public abstract class ChatDecoratorProjection : IChatDecoratorProjection
{
    private readonly IChatDecoratorProjection _projection;

    public ChatDecoratorProjection(IChatDecoratorProjection projection)
    {
        _projection = projection;
    }

    public virtual Task Apply(IChatEventStream @event, CancellationToken cancellation = default) => _projection.Apply(@event);    
}


public class DefauftProjection : IChatDecoratorProjection
{
    public Task Apply(IChatEventStream @event, CancellationToken cancellation = default) => Task.CompletedTask;    
}