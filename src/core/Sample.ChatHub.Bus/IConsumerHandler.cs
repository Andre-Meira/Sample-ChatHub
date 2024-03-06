using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ChatHub.Bus;

public interface IConsumerHandler<T> : IConsumer where T : class
{
    public abstract Task Consumer(IConsumerContext<T> context);
}

public interface IConsumer;
