﻿using RabbitMQ.Client;
using Sample.ChatHub.Bus.Models;

namespace Sample.ChatHub.Bus;

public interface IConsumerOptions
{
    public string ExchangeName { get; set; }
    public string RoutingKey { get; set; }
    public string ExchageType { get; set; }
    public ushort PrefetchCount { get; set; }
    public IFaultConsumerConfiguration? FaultConfig { get; set; } 
}


internal record ConsumerOptions : IConsumerOptions
{
    public string ExchangeName { get; set; } = "";
    public string RoutingKey { get; set; } = "";
    public string ExchageType { get; set; } = ExchangeType.Direct;
    public ushort PrefetchCount { get; set; } = 0;

    public IFaultConsumerConfiguration? FaultConfig { get; set; }
}
