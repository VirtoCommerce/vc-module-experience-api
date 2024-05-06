using System.Threading;
using System.Threading.Tasks;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using Microsoft.Extensions.Options;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Infrastructure
{
    public class KeepAliveResolver : IOperationMessageListener
    {
        private readonly GraphQLWebSocketOptions _webSocketOptions;

        public KeepAliveResolver(IOptions<GraphQLWebSocketOptions> webSoketOptions)
        {
            _webSocketOptions = webSoketOptions.Value;
        }

        private static readonly OperationMessage _keepAliveMessage = new() { Type = MessageType.GQL_CONNECTION_KEEP_ALIVE };

        public Task AfterHandleAsync(MessageHandlingContext context) => Task.CompletedTask;

        public Task BeforeHandleAsync(MessageHandlingContext context) => Task.CompletedTask;

        public Task HandleAsync(MessageHandlingContext context)
        {
            async Task StartKeepAliveLoopAsync(MessageHandlingContext context, CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(_webSocketOptions.KeepAliveInterval, cancellationToken);
                    await context.Writer.SendAsync(_keepAliveMessage);
                }
            }

            switch (context.Message.Type)
            {
                case MessageType.GQL_START:
                    {
                        var cancellationToken = CancellationToken.None;
                        if (context.Subscriptions is CustomSubscriptionManager subscriptions)
                        {
                            var cancellationTokenSource = subscriptions.GetSubscriptionCancellationSource(context.Message.Id);
                            cancellationToken = cancellationTokenSource.Token;
                        }

                        _ = StartKeepAliveLoopAsync(context, cancellationToken);
                        return Task.CompletedTask;
                    }

                default:
                    return Task.CompletedTask;
            }
        }
    }
}
