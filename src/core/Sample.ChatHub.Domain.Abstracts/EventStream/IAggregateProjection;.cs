namespace Sample.ChatHub.Domain.Abstracts.EventStream;

public interface IAggregateProjection
{
    public Guid Id { get; set; }
}


public interface IRepositoreProjection<TProjection> where TProjection : IAggregateProjection
{
    Task<TProjection?> GetAsync(Guid IdProjection, CancellationToken cancellation = default);
    Task ProjectAsync(TProjection projection, CancellationToken cancellation = default);
}