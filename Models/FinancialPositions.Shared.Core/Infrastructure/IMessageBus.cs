using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.Shared.Core.Infrastructure
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, string topic) where T : class;
        Task SubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class;
    }
}
