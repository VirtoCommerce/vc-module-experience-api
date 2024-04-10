using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Server;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using Microsoft.Extensions.Logging;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Infrastructure
{
    /// <summary>
    /// Supports cancellaton of 'keep-alive' message spamming for cancelled subscriptions
    /// </summary>
    public class CustomSubscriptionManager(IGraphQLExecuter executer, ILoggerFactory loggerFactory) : ISubscriptionManager
    {
        private readonly SubscriptionManager _subscriptionManager = new SubscriptionManager(executer, loggerFactory);

        private readonly ConcurrentDictionary<string, CancellationTokenSource> _subscriptionsCancellationSources = new();

        public CancellationTokenSource GetSubscriptionCancellationSource(string id)
        {
            return _subscriptionsCancellationSources.GetOrAdd(id, _ => new CancellationTokenSource());
        }

        public Task SubscribeOrExecuteAsync(string id, OperationMessagePayload payload, MessageHandlingContext context)
        {
            return _subscriptionManager.SubscribeOrExecuteAsync(id, payload, context);
        }

        public Task UnsubscribeAsync(string id)
        {
            if (_subscriptionsCancellationSources.TryRemove(id, out var cancellationTokenSource))
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                }
            }

            return _subscriptionManager.UnsubscribeAsync(id);
        }

        public IEnumerator<Subscription> GetEnumerator()
        {
            return _subscriptionManager.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
