using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ChatHub.Bus;

public interface IConsumerOptions
{
    public string ExchangeName { get; init; }
    public string RoutingKey { get; init; } 
    public string ExchageType { get; init; }
    public ushort PrefetchCount { get; init; }
}


public record ConsumerOptions : IConsumerOptions
{
    public ConsumerOptions(
        string exchangeName, 
        string exchageType, 
        string routingKey = "", 
        ushort prefetchCount = 0)
    {
        ExchangeName = exchangeName;
        RoutingKey = routingKey;
        ExchageType = exchageType;
        PrefetchCount = prefetchCount;
    }

    public string ExchangeName { get; init; }
    public string RoutingKey { get; init; }
    public string ExchageType { get; init; }

    public ushort PrefetchCount { get; init; }
}
