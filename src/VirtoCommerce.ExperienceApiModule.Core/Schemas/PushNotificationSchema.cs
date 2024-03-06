using System;
using System.Linq;
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
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Models;
using VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Schemas;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class PushNotificationSchema(EventBroker eventBroker) : ISchemaBuilder
    {
        private readonly EventBroker _eventBroker = eventBroker;

        private readonly string _commandName = "command";

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
                Type = typeof(PushNotificationType),
                Resolver = new FuncFieldResolver<PushNotification>(ResolveMessage),
                AsyncSubscriber = new AsyncEventStreamResolver<PushNotification>(Subscribe)
            };
            schema.Subscription.AddField(messageAddedEventStreamFieldType);

            var messageAddedToUserEventStreamFieldType = new EventStreamFieldType
            {
                Name = "pushNotificationCreatedForUser",
                Type = typeof(PushNotificationType),
                Resolver = new FuncFieldResolver<PushNotification>(ResolveMessage),
                AsyncSubscriber = new AsyncEventStreamResolver<PushNotification>(SubscribeToUser)
            };
            //schema.Subscription.AddField(messageAddedToUserEventStreamFieldType);

            #region Query
            schema.Query.AddField(new FieldType
            {
                Name = "pushNotifications",
                Arguments = new QueryArguments(
                    new QueryArgument<BooleanGraphType> { Name = "unreadOnly" },
                    new QueryArgument<StringGraphType> { Name = "cultureName" }
                ),
                Type = GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<PushNotificationsResponseType>>(),
                Resolver = new AsyncFieldResolver<object>(context =>
                {
                    var result = new PushNotificationResponse();

                    for (var i = 1; i <= 10; i++)
                    {
                        var message = new PushNotification
                        {
                            Id = Guid.NewGuid().ToString(),
                            ShortMessage = "Test message " + i,
                            Status = i < 5 ? "Unread" : "Read",
                            CreatedDate = DateTime.UtcNow,
                            UserId = context.GetCurrentUserId(),
                        };

                        result.Notifications.Add(message);
                    }

                    result.UnreadCount = result.Notifications.Count(x => x.Status == "Unread");

                    return Task.FromResult<object>(result);
                })
            });
            #endregion

            #region Mutation
            schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                .Name("pushNotificationsClearAll")
                .ResolveAsync(context =>
                {
                    return Task.FromResult(true);
                })
                .FieldType);

            schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                .Name("pushNotificationsMarkReadAll")
                .ResolveAsync(context =>
                {
                    return Task.FromResult(true);

                })
                .FieldType);

            schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                .Name("pushNotificationsMarkUnreadAll")
                .ResolveAsync(context =>
                {
                    return Task.FromResult(true);
                })
                .FieldType);

            schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                .Name("pushNotificationMarkRead")
                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputMarkPushNotificationReadType>>(), _commandName)
                .ResolveAsync(context =>
                {
                    return Task.FromResult(true);
                })
            .FieldType);

            schema.Mutation.AddField(FieldBuilder.Create<object, bool>(typeof(BooleanGraphType))
                .Name("pushNotificationMarkUnread")
                .Argument(GraphTypeExtenstionHelper.GetActualComplexType<NonNullGraphType<InputMarkPushNotificationUnreadType>>(), _commandName)
                .ResolveAsync(context =>
                {
                    return Task.FromResult(true);
                })
                .FieldType);
            #endregion
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
