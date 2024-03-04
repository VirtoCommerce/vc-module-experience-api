using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using GraphQL.Subscription;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class EventSchema(EventBroker eventBroker) : ISchemaBuilder
    {
        private readonly EventBroker _eventBroker = eventBroker;

        public void Build(ISchema schema)
        {
            var addMessageFieldType = FieldBuilder.Create<object, PushNotification>(GraphTypeExtenstionHelper.GetActualType<PushNotificationType>())
                            .Name("addMessage")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<PushNotificationInputType>>(), "command")
                            .ResolveAsync(async context =>
                            {
                                var command = context.GetArgument<AddMessageCommand>("command");

                                var message = new PushNotification
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Content = command.Content,
                                    SentDate = DateTime.UtcNow,
                                    UserId = context.GetCurrentUserId(),
                                };

                                await _eventBroker.AddMessageAsync(message);

                                return message;
                            })
                            .FieldType;
            schema.Mutation.AddField(addMessageFieldType);

            var messageEventStreamFieldType = new EventStreamFieldType
            {
                Name = "messageAdded",
                Type = typeof(PushNotificationType),
                Resolver = new FuncFieldResolver<PushNotification>(ResolveMessage),
                AsyncSubscriber = new AsyncEventStreamResolver<PushNotification>(Subscribe)
            };
            schema.Subscription.AddField(messageEventStreamFieldType);
        }

        private PushNotification ResolveMessage(IResolveFieldContext context)
        {
            var message = context.Source as PushNotification;

            return message;
        }

        private Task<IObservable<PushNotification>> Subscribe(IResolveEventStreamContext context)
        {
            var messageContext = (MessageHandlingContext)context.UserContext;

            var result = _eventBroker.MessagesAsync();

            return result;
        }
    }
}
