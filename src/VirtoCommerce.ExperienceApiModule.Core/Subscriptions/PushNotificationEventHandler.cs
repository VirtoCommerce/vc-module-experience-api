//using System.Threading.Tasks;
//using VirtoCommerce.NotificationEvent.Core.Events;
//using VirtoCommerce.Platform.Core.Events;

//namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
//{
//    // FOR TESTING PURPOSES ONLY
//    public class PushNotificationEventHandler : IEventHandler<PushNotificationCreatedEvent>
//    {
//        private readonly EventBroker _eventBroker;

//        public PushNotificationEventHandler(EventBroker eventBroker)
//        {
//            _eventBroker = eventBroker;
//        }

//        public async Task Handle(PushNotificationCreatedEvent message)
//        {
//            foreach (var entry in message.ChangedEntries)
//            {
//                var notification = entry.NewEntry;

//                await _eventBroker.AddMessageAsync(notification);
//            }
//        }
//    }
//}
