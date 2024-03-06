namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkPushNotificationUnreadCommand : PushNotificationsCommand
    {
        public string NotificationId { get; set; }
    }
}
