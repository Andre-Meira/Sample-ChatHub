namespace Sample.ChatHub.Bus;


[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class ContractAttribute : Attribute
{
    public ContractAttribute(string exchange = "", 
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


public static class ContractExtensions
{
    public static string GetExchangeContract<IContract>()
    {
        Type messageType = typeof(IContract);
        ContractAttribute? infoAttribute = (ContractAttribute?)Attribute.GetCustomAttribute(messageType, typeof(ContractAttribute));

        if(infoAttribute == null)
            throw new ArgumentException("O contrato não possui o atributo ContractAttribute");
            
        return infoAttribute.Exchange;
    }

    public static string GetExchangeTypeContract<IContract>()
    {
        Type messageType = typeof(IContract);
        ContractAttribute? infoAttribute = (ContractAttribute?)Attribute.GetCustomAttribute(messageType, typeof(ContractAttribute));

        if(infoAttribute == null)
            throw new ArgumentException("O contrato não possui o atributo ContractAttribute");
            
        return infoAttribute.ExchangeType;
    }

}
