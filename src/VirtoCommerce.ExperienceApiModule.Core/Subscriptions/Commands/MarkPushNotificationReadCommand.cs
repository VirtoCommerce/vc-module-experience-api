namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkPushNotificationReadCommand : PushNotificationsCommand
    {
        public string NotificationId { get; set; }
    }
}
