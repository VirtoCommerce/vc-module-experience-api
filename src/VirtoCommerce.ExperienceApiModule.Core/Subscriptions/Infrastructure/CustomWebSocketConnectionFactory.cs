using System.Collections.Generic;
using System.Net.WebSockets;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Server.Transports.WebSockets;
using GraphQL.Types;
using Microsoft.Extensions.Logging;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Infrastructure
{
    /// <summary>
    /// Needed to replace default SubscriptionManager with custom implementation
    /// </summary>
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
}
