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
    public class PushMessagesSchema(IEventBroker eventBroker) : ISchemaBuilder
    {
        private readonly IEventBroker _eventBroker = eventBroker;

        public void Build(ISchema schema)
        {
            var addMessageFieldType = FieldBuilder.Create<object, ExpPushMessage>(GraphTypeExtenstionHelper.GetActualType<PushMessageType>())
                            .Name("addMessage")
                            .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<PushNotificationInputType>>(), "command")
                            .ResolveAsync(async context =>
                            {
                                var command = context.GetArgument<AddMessageCommand>("command");

                                var message = new ExpPushMessage
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
                Name = "pushMessageCreated",
                Type = typeof(PushMessageType),
                Resolver = new FuncFieldResolver<ExpPushMessage>(ResolveMessage),
                AsyncSubscriber = new AsyncEventStreamResolver<ExpPushMessage>(Subscribe)
            };
            schema.Subscription.AddField(messageAddedEventStreamFieldType);

            //var messageAddedToUserEventStreamFieldType = new EventStreamFieldType
            //{
            //    Name = "pushNotificationCreatedForUser",
            //    Type = typeof(PushMessageType),
            //    Resolver = new FuncFieldResolver<PushMessage>(ResolveMessage),
            //    AsyncSubscriber = new AsyncEventStreamResolver<PushMessage>(SubscribeToUser)
            //};
            //schema.Subscription.AddField(messageAddedToUserEventStreamFieldType);
        }

        private ExpPushMessage ResolveMessage(IResolveFieldContext context)
        {
            return context.Source as ExpPushMessage;
        }

        private Task<IObservable<ExpPushMessage>> Subscribe(IResolveEventStreamContext context)
        {
            return _eventBroker.MessagesAsync();
        }

        //private Task<IObservable<PushMessage>> SubscribeToUser(IResolveEventStreamContext context)
        //{
        //    var currentUserId = context.GetCurrentUserId();

        //    var result = _eventBroker.MessagesByUserIdAsync(currentUserId);

        //    return result;
        //}
    }
}
