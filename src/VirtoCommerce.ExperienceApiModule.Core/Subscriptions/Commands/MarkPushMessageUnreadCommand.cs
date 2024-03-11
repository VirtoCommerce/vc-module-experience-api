namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Commands
{
    public class MarkPushMessageUnreadCommand : PushMessagesCommand
    {
        public string NotificationId { get; set; }
    }
}
