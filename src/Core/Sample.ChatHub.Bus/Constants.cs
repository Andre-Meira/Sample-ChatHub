namespace Sample.ChatHub.Bus;

internal class ActivityConstants
{
    public const string Source = "MessageBroker";

    public const string Header_Id = "activity_id";
    public const string Header_TraceId = "activity_trace_id";
    public const string Header_SpanId = "activity_span_id";
    public const string Header_TraceFlags = "activity_trace_flags";
    public const string Header_TraceState = "activity_trace_state";

    public const string Hostname = "hostname";
    public const string Id = "id";
    public const string TraceId = "trace.id";
    public const string SpanId = "span.id";

    public const string MessageName = "message.name";
    public const string MessageRouting = "message.routing";
    public const string MessageExchange = "message.exchange";
    public const string MessageExchageType = "message.exchageType";

    public const string NameActivity_Consumer = "Consumer-Message";
    public const string NameActivity_Publish = "Publish-Message";

    public const string ExceptionMessage = "exception.message";
    public const string ExceptionType = "exception.type";
    public const string ExceptionStackTrace = "exception.stacktrace";

}

