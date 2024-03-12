using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Server.Transports.Subscriptions.Abstractions;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Infrastructure
{
    public class KeepAliveResolver : IOperationMessageListener
    {
        public Task AfterHandleAsync(MessageHandlingContext context) => Task.CompletedTask;

        public Task BeforeHandleAsync(MessageHandlingContext context) => Task.CompletedTask;

        private static readonly OperationMessage _keepAliveMessage = new() { Type = MessageType.GQL_CONNECTION_KEEP_ALIVE };

        public Task HandleAsync(MessageHandlingContext context)
        {
            async Task StartKeepAliveLoopAsync(MessageHandlingContext context)
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(31), CancellationToken.None);
                    await context.Writer.SendAsync(_keepAliveMessage);
                }
            }

            switch (context.Message.Type)
            {
                case MessageType.GQL_CONNECTION_INIT:
                    {
                        _ = StartKeepAliveLoopAsync(context);
                        return Task.CompletedTask;
                    }

                default:
                    return Task.CompletedTask;
            }
        }
    }
}
