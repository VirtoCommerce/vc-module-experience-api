using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Server.Transports.WebSockets;
using GraphQL.Types;
using Microsoft.Extensions.Logging;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Infrastructure
{
    public class CustomWebSocketConnectionFactory<TSchema>(ILogger<WebSocketConnectionFactory<TSchema>> logger,
        ILoggerFactory loggerFactory,
        IGraphQLExecuter<TSchema> executer,
        IEnumerable<IOperationMessageListener> messageListeners,
        IDocumentWriter documentWriter) : IWebSocketConnectionFactory<TSchema>
        where TSchema : ISchema
    {
        private readonly ILogger _logger = logger;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly IGraphQLExecuter<TSchema> _executer = executer;
        private readonly IEnumerable<IOperationMessageListener> _messageListeners = messageListeners;
        private readonly IDocumentWriter _documentWriter = documentWriter;

        public WebSocketConnection CreateConnection(WebSocket socket, string connectionId)
        {
            _logger.LogDebug("Creating server for connection {connectionId}", connectionId);

            var transport = new WebSocketTransport(socket, _documentWriter);
            var manager = new CustomSubscriptionManager(_executer, _loggerFactory);
            var server = new SubscriptionServer(
                transport,
                manager,
                _messageListeners,
                _loggerFactory.CreateLogger<SubscriptionServer>()
            );

            return new WebSocketConnection(transport, server);
        }
    }

    public class CustomSubscriptionManager(IGraphQLExecuter executer, ILoggerFactory loggerFactory) : ISubscriptionManager
    {
        private readonly SubscriptionManager _subscriptionManager = new SubscriptionManager(executer, loggerFactory);

        private readonly ConcurrentDictionary<string, CancellationTokenSource> _subscriptionsCancellationSources = new();

        public void RegisterSubscriptionCancellationSource(string id, CancellationTokenSource cancellationTokenSource)
        {
            _subscriptionsCancellationSources.TryAdd(id, cancellationTokenSource);
        }

        public CancellationTokenSource GetSubscriptionCancellationSource(string id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            _subscriptionsCancellationSources.TryAdd(id, cancellationTokenSource);
            return cancellationTokenSource;
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
