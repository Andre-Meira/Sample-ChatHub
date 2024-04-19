using System.Diagnostics;

namespace Sample.ChatHub.Bus.Monitory;

public readonly struct ActivityBus
{
    private readonly Activity _activity;
    public ActivityBus(Activity activity) => _activity = activity;

    public void SetTag(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        _activity.SetTag(key, value);
    }    

    public void AddExceptionEvent(Exception exception)
    {
        exception = exception.GetBaseException() ?? exception;

        var exceptionMessage = exception.Message;

        var tags = new ActivityTagsCollection
            {                
                { "exceptions.message", exceptionMessage },
                { "exception.type", exception.GetType().Name },
                { "exception.stacktrace", exception.StackTrace }
            };

        var activityEvent = new ActivityEvent("exception", DateTimeOffset.UtcNow, tags);

        _activity.AddEvent(activityEvent);
        _activity.SetStatus(ActivityStatusCode.Error, exceptionMessage);
    }

    public void Stop()
    {
        if (_activity.Status == ActivityStatusCode.Unset)
            _activity.SetStatus(ActivityStatusCode.Ok);

        _activity.Dispose();
    }

    public void Start() => _activity.Start();
}


