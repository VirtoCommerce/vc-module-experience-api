using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
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

            var messageAddedEventStreamFieldType = new EventStreamFieldType
            {
                Name = "messageAdded",
                Type = typeof(PushNotificationType),
                Resolver = new FuncFieldResolver<PushNotification>(ResolveMessage),
                AsyncSubscriber = new AsyncEventStreamResolver<PushNotification>(Subscribe)
            };
            schema.Subscription.AddField(messageAddedEventStreamFieldType);

            var messageAddedToUserEventStreamFieldType = new EventStreamFieldType
            {
                Name = "messageAddedToUser",
                Type = typeof(PushNotificationType),
                Resolver = new FuncFieldResolver<PushNotification>(ResolveMessage),
                AsyncSubscriber = new AsyncEventStreamResolver<PushNotification>(SubscribeToUser)
            };
            schema.Subscription.AddField(messageAddedToUserEventStreamFieldType);
        }

        private PushNotification ResolveMessage(IResolveFieldContext context)
        {
            return context.Source as PushNotification;
        }

        private Task<IObservable<PushNotification>> Subscribe(IResolveEventStreamContext context)
        {
            return _eventBroker.MessagesAsync();
        }

        private Task<IObservable<PushNotification>> SubscribeToUser(IResolveEventStreamContext context)
        {
            var currentUserId = context.GetCurrentUserId();

            var result = _eventBroker.MessagesByUserIdAsync(currentUserId);

            return result;
        }
    }
}
