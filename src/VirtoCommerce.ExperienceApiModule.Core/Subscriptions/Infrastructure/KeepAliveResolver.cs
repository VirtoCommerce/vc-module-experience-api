using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Server.Transports.Subscriptions.Abstractions;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Infrastructure
{
    public class KeepAliveResolver : IOperationMessageListener
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        private static readonly OperationMessage _keepAliveMessage = new() { Type = MessageType.GQL_CONNECTION_KEEP_ALIVE };

        public KeepAliveResolver()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task AfterHandleAsync(MessageHandlingContext context) => Task.CompletedTask;

        public Task BeforeHandleAsync(MessageHandlingContext context) => Task.CompletedTask;

        public Task HandleAsync(MessageHandlingContext context)
        {
            async Task StartKeepAliveLoopAsync(MessageHandlingContext context, CancellationToken cancellationToken)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                    await context.Writer.SendAsync(_keepAliveMessage);
                }
            }

            switch (context.Message.Type)
            {
                case MessageType.GQL_START:
                    {
                        if (context.Subscriptions is CustomSubscriptionManager subscriptions)
                        {
                            subscriptions.RegisterSubscriptionCancellationSource(context.Message.Id, _cancellationTokenSource);
                        }

                        _ = StartKeepAliveLoopAsync(context, _cancellationTokenSource.Token);
                        return Task.CompletedTask;
                    }

                default:
                    return Task.CompletedTask;
            }
        }
    }
}
