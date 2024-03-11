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
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas
{
    public class PushMessagesSchema(EventBroker eventBroker) : ISchemaBuilder
    {
        private readonly EventBroker _eventBroker = eventBroker;

        public void Build(ISchema schema)
        {
            var addMessageFieldType = FieldBuilder.Create<object, PushMessage>(GraphTypeExtenstionHelper.GetActualType<PushMessageType>())
                            .Name("addMessage")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<PushNotificationInputType>>(), "command")
                            .ResolveAsync(async context =>
                            {
                                var command = context.GetArgument<AddMessageCommand>("command");

                                var message = new PushMessage
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    ShortMessage = command.Content,
                                    CreatedDate = DateTime.UtcNow,
                                    UserId = context.GetCurrentUserId(),
                                };

                                await _eventBroker.AddMessageAsync(message);

                                return message;
                            })
                            .FieldType;
            schema.Mutation.AddField(addMessageFieldType);

            var messageAddedEventStreamFieldType = new EventStreamFieldType
            {
                Name = "pushNotificationCreated",
                Type = typeof(PushMessageType),
                Resolver = new FuncFieldResolver<PushMessage>(ResolveMessage),
                AsyncSubscriber = new AsyncEventStreamResolver<PushMessage>(Subscribe)
            };
            schema.Subscription.AddField(messageAddedEventStreamFieldType);

            var messageAddedToUserEventStreamFieldType = new EventStreamFieldType
            {
                Name = "pushNotificationCreatedForUser",
                Type = typeof(PushMessageType),
                Resolver = new FuncFieldResolver<PushMessage>(ResolveMessage),
                AsyncSubscriber = new AsyncEventStreamResolver<PushMessage>(SubscribeToUser)
            };
            //schema.Subscription.AddField(messageAddedToUserEventStreamFieldType);
        }

        private PushMessage ResolveMessage(IResolveFieldContext context)
        {
            return context.Source as PushMessage;
        }

        private Task<IObservable<PushMessage>> Subscribe(IResolveEventStreamContext context)
        {
            return _eventBroker.MessagesAsync();
        }

        private Task<IObservable<PushMessage>> SubscribeToUser(IResolveEventStreamContext context)
        {
            var currentUserId = context.GetCurrentUserId();

            var result = _eventBroker.MessagesByUserIdAsync(currentUserId);

            return result;
        }
    }
}
