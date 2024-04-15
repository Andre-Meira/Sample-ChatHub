namespace Sample.ChatHub.Bus;


[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class MessageAttribute : Attribute
{
    public MessageAttribute(string exchange = "",
        string exchangeType = "direct",
        string routingKey = "")
    {
        Exchange = exchange;
        ExchangeType = exchangeType;
        RoutingKey = routingKey;
    }

    public string Exchange { get; }

    public string ExchangeType { get; }

    public string RoutingKey { get; }


}
