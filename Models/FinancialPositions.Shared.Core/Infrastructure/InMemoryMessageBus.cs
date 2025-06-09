using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.Shared.Core.Infrastructure
{
    public class InMemoryMessageBus : IMessageBus
    {
        private readonly ConcurrentDictionary<string, List<Func<object, Task>>> _handlers = new ConcurrentDictionary<string, List<Func<object, Task>>>();
        public async Task PublishAsync<T>(T message, string topic) where T : class
        {
            if (_handlers.TryGetValue(topic, out var handlers))
            {
                var tasks = handlers.Select(handler => handler(message));
                await Task.WhenAll(tasks);
            }
        }
        public Task SubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class
        {
            _handlers.AddOrUpdate(topic, new List<Func<object, Task>> { async obj => await handler((T)obj) },
            (_, list) =>
            {
                list.Add(async obj => await handler((T)obj));
                return list;
            });
            return Task.CompletedTask;
        }
    }
}
